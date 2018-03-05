/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Log;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Packages.DataProvider {
    // not a real data provider - used to clear/create all package data and initial web pages
    public partial class PackagesDataProvider {

        public string TemplateFolder {
            get {
                string rootFolder;
#if MVC6
                rootFolder = YetaWFManager.RootFolderWebProject;
#else
                rootFolder = YetaWFManager.RootFolder;
#endif
                return Path.Combine(rootFolder, Globals.SiteTemplates);
            }
        }
        public string DataFolderName { get { return "Data"; } }

        private InternalError TemplateError(string message, params object[] parms) {
            return TemplateError(string.Format(message, parms));
        }
        private InternalError TemplateError(string message) {
            return new InternalError(string.Format("{0}({1}): {2}", _Template, _LineCounter, message));
        }

        private int _LineCounter;
        private string _Template;
        private MenuList _SiteMenu;
        private PageDefinition _CurrentPage;
        private string _CurrentUrl;

        /// <summary>
        /// Builds the current site from the template file
        /// </summary>
        /// <returns></returns>
        public async Task BuildSiteUsingTemplateAsync(string template, bool build = true) {
            Logging.AddLog("Running site template {0}", template);
            // Get the current site menu
            ModuleDefinition menuServices = ModuleDefinition.Load(Manager.CurrentSite.MenuServices, AllowNone: true);
            if (menuServices == null)
                throw TemplateError("No menu services available - no module has been defined in Site Settings");
            IModuleMenuAsync iModMenu = (IModuleMenuAsync)menuServices;
            if (iModMenu == null)
                throw TemplateError($"Menu services module doesn't implement {nameof(IModuleMenuAsync)} interface");

            _SiteMenu = await iModMenu.GetMenuAsync();
            Manager.SiteCreationTemplateActive = true;
            BuildSite(template, build);
            Manager.SiteCreationTemplateActive = false;

            // Save the new site menu
            await iModMenu.SaveMenuAsync(_SiteMenu);
        }

        /// <summary>
        /// Builds the current site from the template file
        /// </summary>
        /// <returns></returns>
        private void BuildSite(string template, bool build) {

            string initDataFile = Path.Combine(TemplateFolder, template);

            List<string> lines = new List<string>(File.ReadAllLines(initDataFile));
            // substitute all variables (if any)
            Variables vars = new Variables(Manager) { DoubleEscape = true, CurlyBraces = false };
            List<string> newLines = new List<string>();
            foreach (string line in lines) {
                string l = vars.ReplaceVariables(line);// variable substitution
                newLines.Add(l);
            }
            // process lines
            int oldLineCounter = _LineCounter;
            string oldTemplate = _Template;
            _LineCounter = 0;
            _Template = template;
            BuildSiteFromText(newLines, build);
            _LineCounter = oldLineCounter;
            _Template = oldTemplate;
        }

        /// <summary>
        /// Builds the current site from the template file
        /// </summary>
        /// <returns></returns>
        private void BuildSiteFromText(List<string> lines, bool build) {
            // Build site from a template
            if (lines.Count() > 0) {
                lines = RemoveComments(lines);
                // process all lines
                for (; lines.Count > 0;) {
                    string section = lines.First().Trim(); ++_LineCounter; lines.RemoveAt(0);
                    if (section == "")
                        continue;
                    else if (section == "::PAGE-SECTION::")
                        ExtractPageSection(lines, build);
                    else if (section == "::UNIQUE-MODULE-SECTION::")
                        ExtractUniqueModuleSection(lines, build);
                    else if (section == "::NON-UNIQUE-MODULE-SECTION::")
                        ExtractUniqueModuleSection(lines, build, Unique: false);
                    else if (section == "::LINK-SECTION::")
                        ExtractLinkSection(lines, build);
                    else if (section == "::WEBCONFIG-SECTION::")
                        ExtractWebconfigSection(lines, build);
                    else if (section == "::COMMAND-SECTION::")
                        ExtractCommandSection(lines, build);
                    else if (section == "::SQL-SECTION::")
                        ExtractSQLSection(lines, build);
                    else if (section.StartsWith("::INC "))
                        ExtractINCSection(lines, build, section);
                    else
                        throw TemplateError("Invalid section statement");
                }
            } else {
                if (build) {
                    // Just build a site with a home page
                    // Build a site with a few pages
                    //PageDefinition page = new PageDefinition() { Title = "Home Page", Url = "/", PageGuid = new Guid(Globals.HomePageGuid) };
                    PageDefinition page = PageDefinition.CreatePageDefinition("/");
                    page.Save();
#if DEBUG
                    page = PageDefinition.CreatePageDefinition("/page1");
                    page.Save();
                    page = PageDefinition.CreatePageDefinition("/page2");
                    page.Save();
                    page = PageDefinition.CreatePageDefinition("/page3");
                    page.Save();
#endif
                }
            }
        }

        /// <summary>
        /// Remove all comments
        /// </summary>
        private List<string> RemoveComments(List<string> lines) {
            List<string> nl = new List<string>();
            foreach (string l in lines) {
                string line = l.Replace("\t", "    ");
                int i = line.IndexOf("###");
                if (i >= 0)
                    nl.Add(line.Substring(0, i));
                else
                    nl.Add(line);
            }
            return nl;
        }

        /// <summary>
        /// Remove all lines that contain the specified tag (set the line to "")
        /// </summary>
        private List<string> RemoveLines(List<string> lines, string tag, bool remove) {
            List<string> nl = new List<string>();
            if (remove) {
                foreach (string l in lines) {
                    if (l.Contains(tag))
                        nl.Add("");
                    else
                        nl.Add(l);
                }
            } else {
                tag += " ";
                foreach (string l in lines) {
                    nl.Add(l.Replace(tag, ""));
                }
            }
            return nl;
        }

        private void ExtractPageSection(List<string> lines, bool build) {

            for (; lines.Count > 0;) {
                // get the menu path (site url)
                string urlLine = lines.First();
                if (urlLine.StartsWith("::"))
                    break; // start of a new section

                ++_LineCounter; lines.RemoveAt(0);
                if (urlLine.Trim() == "")
                    continue;
                string[] s = urlLine.Split(new char[] { ' ' }, 2);
                if (s.Length != 2)
                    throw TemplateError("Url {0} invalid", urlLine);

                string user = s[0].Trim();
                string url = s[1].Trim();

                // validate url
                if (url.Length == 0 || url[0] != '/')
                    throw TemplateError("Url must start with / and can't be indented");

                PageDefinition page = null;
                if (build) {
                    page = PageDefinition.CreatePageDefinition(url);
                } else {
                    page = PageDefinition.LoadFromUrl(url);
                    if (page != null)
                        PageDefinition.RemovePageDefinition(page.PageGuid);
                    page = new PageDefinition();
                }
                _CurrentPage = page;
                _CurrentUrl = url;

                while (TryPageTitle(page, lines) || TryPageDescription(page, lines))
                    ;

                // Get the Menu Text
                if (lines.Count < 1)
                    throw TemplateError("Menu missing");
                string menu = lines.First(); ++_LineCounter; lines.RemoveAt(0);
                if (string.IsNullOrWhiteSpace(menu))
                    throw TemplateError("Menu missing");
                if (!char.IsWhiteSpace(menu[0]))
                    throw TemplateError("Menu line must be indented");
                menu = menu.Trim();

                // add a menu entry to the site menu
                if (menu != "-") {
                    if (build) {
                        ModuleAction action = AddMenu(menu);
                        //if (string.IsNullOrWhiteSpace(action.Url)) // I don't remember why this was here
                        action.Url = url;
                    } else {
                        RemoveMenu(menu);
                    }
                }

                // Get the skin
                if (lines.Count < 1)
                    throw TemplateError("Skin missing");
                string skin = lines.First(); ++_LineCounter; lines.RemoveAt(0);
                if (string.IsNullOrWhiteSpace(skin))
                    throw TemplateError("Skin missing");
                if (!char.IsWhiteSpace(skin[0]))
                    throw TemplateError("Skin line must be indented");
                skin = skin.Trim();
                if (skin == "-")
                    skin = ",,,";
                string[] skinparts = skin.Split(new char[] { ',' });
                if (skinparts.Length != 4)
                    throw TemplateError("Invalid skin format");

                page.SelectedSkin.Collection = skinparts[0].Trim();
                if (page.SelectedSkin.Collection == "-")
                    page.SelectedSkin.Collection = null;
                page.SelectedSkin.FileName = skinparts[1].Trim();
                page.SelectedPopupSkin.Collection = skinparts[2].Trim();
                if (page.SelectedPopupSkin.Collection == "-")
                    page.SelectedPopupSkin.Collection = null;
                page.SelectedPopupSkin.FileName = skinparts[3].Trim();

                SerializableList<PageDefinition.AllowedRole> roles = new SerializableList<PageDefinition.AllowedRole>();
                switch (user) {
                    case "Administrator":
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetAdministratorRoleId(), View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, Remove = PageDefinition.AllowedEnum.Yes, });
                        break;
                    case "User":
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetUserRoleId(), View = PageDefinition.AllowedEnum.Yes, });
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetEditorRoleId(), View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, });
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetAdministratorRoleId(), View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, Remove = PageDefinition.AllowedEnum.Yes, });
                        break;
                    case "Anonymous":
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetAnonymousRoleId(), View = PageDefinition.AllowedEnum.Yes, });
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetEditorRoleId(), View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, });
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetAdministratorRoleId(), View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, Remove = PageDefinition.AllowedEnum.Yes, });
                        break;
                    case "Anonymous+User":
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetAnonymousRoleId(), View = PageDefinition.AllowedEnum.Yes, });
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetUserRoleId(), View = PageDefinition.AllowedEnum.Yes, });
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetEditorRoleId(), View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, });
                        roles.Add(new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetAdministratorRoleId(), View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, Remove = PageDefinition.AllowedEnum.Yes, });
                        break;
                    default:
                        throw TemplateError("User {0} invalid", user);
                }
                page.AllowedRoles = roles;

                // Get zero, one or more modules (add to Main section)
                for (; lines.Count > 0;) {

                    string modsLine = lines.First();
                    if (string.IsNullOrWhiteSpace(modsLine)) {
                        ++_LineCounter; lines.RemoveAt(0);
                        continue;
                    }
                    if (!char.IsWhiteSpace(modsLine[0]))
                        break;// not a module - module lines must be indented

                    // Load assembly
                    ++_LineCounter; lines.RemoveAt(0);// accept as module line
                    string[] parts = modsLine.Split(new char[] { ':' });
                    string pane = null;
                    string modDef = null;
                    if (parts.Length == 1) {
                        pane = Globals.MainPane;
                        modDef = parts[0].Trim();
                    } else if (parts.Length == 2) {
                        pane = parts[0].Trim();
                        if (pane == "-")
                            pane = Globals.MainPane;
                        modDef = parts[1].Trim();
                    } else
                        throw TemplateError("Invalid \"pane: module definition\"");
                    ModuleDefinition mod = EvaluateModuleAsmTypeExpression(modDef);
                    page.AddModule(pane, mod);

                    for (; lines.Count > 0;) {
                        // add variable customizations (if any)
                        string varLine = lines.First();
                        if (string.IsNullOrWhiteSpace(varLine)) {
                            ++_LineCounter; lines.RemoveAt(0);
                            continue;
                        }
                        // if this line is more indented than the module line, it must be a variable line
                        if (varLine.Length - varLine.TrimStart(new char[] { ' ' }).Length <= modsLine.Length - modsLine.TrimStart(new char[] { ' ' }).Length)
                            break;
                        // process variables
                        ++_LineCounter; lines.RemoveAt(0);// accept as variable line
                        if (varLine.Trim() == "") continue;
                        string[] sv = varLine.Split(new char[] { '=' }, 2);
                        if (varLine.Trim().EndsWith(")")) {// method call
                            varLine = varLine.Trim();
                            if (build)
                                InvokeMethod(varLine, varLine, mod);
                        } else if (sv.Length >= 2) { // variable assignment
                            if (build)
                                AssignVariable(mod, sv[0].Trim(), sv[1].Trim());
                        } else
                            throw TemplateError("Variable assignment invalid");
                    }
                }
                if (build)
                    page.Save();
                _CurrentPage = null;
                _CurrentUrl = null;
            }
        }

        private bool TryPageDescription(PageDefinition page, List<string> lines) {
            if (lines.Count() <= 0) return false;
            string line = lines.First();
            string[] parts = line.Split(new char[] { '=' }, 2);
            if (parts.Length < 1) return false;
            string cmd = parts[0].Trim();
            if (string.IsNullOrWhiteSpace(cmd)) return false;
            if (cmd != "Description") return false;
            page.Description = parts[1].Trim();
            ++_LineCounter; lines.RemoveAt(0);
            return true;
        }

        private bool TryPageTitle(PageDefinition page, List<string> lines) {
            if (lines.Count() <= 0) return false;
            string line = lines.First();
            string[] parts = line.Split(new char[] { '=' }, 2);
            if (parts.Length < 1) return false;
            string cmd = parts[0].Trim();
            if (string.IsNullOrWhiteSpace(cmd)) return false;
            if (cmd != "Title") return false;
            page.Title = page.Description = parts[1].Trim();
            ++_LineCounter; lines.RemoveAt(0);
            return true;
        }

        private void ExtractUniqueModuleSection(List<string> lines, bool build, bool Unique = true) {

            for (; lines.Count > 0;) {
                // get the module definition
                string modsLine = lines.First();
                if (modsLine.StartsWith("::"))
                    break; // start of a new section

                // Load assembly
                ++_LineCounter; lines.RemoveAt(0);// accept as module line
                if (string.IsNullOrWhiteSpace(modsLine))
                    continue;
                ModuleDefinition mod = EvaluateModuleAsmTypeExpression(modsLine, UniqueMod: Unique);
                if (Unique) {
                    if (!mod.IsModuleUnique)
                        throw TemplateError("Can't use a non-unique module here");
                } else {
                    if (mod.IsModuleUnique)
                        throw TemplateError("Can't use a unique module here");
                }
                mod.Temporary = false;

                for (; lines.Count > 0;) {
                    // add variable customizations (if any)
                    string varLine = lines.First();
                    if (string.IsNullOrWhiteSpace(varLine)) {
                        ++_LineCounter; lines.RemoveAt(0);
                        continue;
                    }
                    // if this line is more indented than the module line, it must be a variable line
                    if (varLine.Length - varLine.TrimStart(new char[] { ' ' }).Length <= modsLine.Length - modsLine.TrimStart(new char[] { ' ' }).Length)
                        break;
                    // process variables
                    ++_LineCounter; lines.RemoveAt(0);// accept as variable line
                    string[] sv = varLine.Split(new char[] { '=' }, 2);
                    if (varLine.Trim().EndsWith(")")) {// method call
                        varLine = varLine.Trim();
                        InvokeMethod(varLine, varLine, mod);
                    } else if (sv.Length == 2) {// variable assignment
                        AssignVariable(mod, sv[0].Trim(), sv[1].Trim());
                    } else
                        throw TemplateError("Variable assignment invalid");
                }
                if (build)
                    mod.Save();
            }
        }
        private ModuleDefinition EvaluateModuleAsmTypeExpression(string modsText, bool UniqueMod = false) {
            object obj = EvaluateObjectAsmTypeExpression(modsText, UniqueMod: UniqueMod);
            if (obj is ModuleDefinition)
                return (ModuleDefinition)obj;
            throw new InternalError("Object doesn't evaluate to a module definition");
        }
        private object EvaluateObjectAsmTypeExpression(string objText, bool UniqueMod = false) {
            string[] s = objText.Split(new char[] { ',' });
            if (s.Length != 2)
                throw TemplateError("Object {0} invalid (need assembly, type)", objText);
            string assembly = s[0].Trim();
            if (string.IsNullOrWhiteSpace(assembly))
                throw TemplateError("Assembly missing");
            string type = s[1].Trim();
            if (string.IsNullOrWhiteSpace(type))
                throw TemplateError("Type missing");

            try {
                Assembly asm = Assemblies.Load(assembly);
                Type tp = asm.GetType(type);
                if (UniqueMod)
                    return ModuleDefinition.CreateUniqueModule(tp);
                else
                    return Activator.CreateInstance(tp);
            } catch (Exception exc) {
                throw TemplateError("Can't create object {0}, {1}", assembly, type, exc.Message);
            }
        }

        private void AssignVariable(object obj, string varName, string value) {
            string[] vars = varName.Split(new char[] { '.' });
            varName = vars[0].Trim();
            value = value.Replace("\\r\\n", Environment.NewLine);
            PropertyInfo pi = ObjectSupport.GetProperty(obj.GetType(), varName);
            if (vars.Length > 1) {
                obj = pi.GetValue(obj);
                AssignVariable(obj, string.Join(".", vars, 1, vars.Length - 1), value);
            } else {
                Type propType = pi.PropertyType;
                if (propType == typeof(string)) {
                    value = TrimQuotes(value);
                    pi.SetValue(obj, value);
                } else if (propType == typeof(int)) {
                    pi.SetValue(obj, Convert.ToInt32(value));
                } else if (propType.BaseType != null && propType.BaseType == typeof(System.Enum)) {
                    pi.SetValue(obj, Convert.ToInt32(value));
                } else if (propType == typeof(MultiString)) {
                    pi.SetValue(obj, new MultiString(TrimQuotes(value)));
                } else if (propType == typeof(bool)) {
                    bool boolVal;
                    if (value.ToLower() == "true")
                        boolVal = true;
                    else if (value.ToLower() == "false")
                        boolVal = false;
                    else if (value == "1")
                        boolVal = true;
                    else if (value == "0")
                        boolVal = false;
                    else
                        throw TemplateError("Invalid bool value {0} for variable {1}", value, varName);
                    pi.SetValue(obj, boolVal);
                } else
                    throw TemplateError("Unsupported type {0} for variable {1}", propType.FullName, varName);
            }
        }

        private static string TrimQuotes(string value) {
            int len = value.Length;
            if (len >= 2) {// trim quotes (if any)
                if (value[0] == '"' && value[len - 1] == '"')
                    value = value.Substring(1, len - 2);
            }
            return value;
        }

        private void ExtractLinkSection(List<string> lines, bool build) {

            for (; lines.Count > 0;) {
                // get the url
                string url = lines.First();
                if (url.StartsWith("::"))
                    break; // start of a new section

                ++_LineCounter; lines.RemoveAt(0);
                if (url.Trim() == "")
                    continue;
                if (url[0] != '/' && !url.StartsWith("http"))
                    throw TemplateError("Url must start with / or http and can't be indented");
                url = url.Trim();

                // Get the Menu Text
                if (lines.Count < 1)
                    throw TemplateError("menu missing");
                string menu = lines.First(); ++_LineCounter; lines.RemoveAt(0);
                if (string.IsNullOrWhiteSpace(menu))
                    throw TemplateError("Menu missing");
                if (!char.IsWhiteSpace(menu[0]))
                    throw TemplateError("Menu line must be indented");
                menu = menu.Trim();

                if (build) {
                    ModuleAction action = AddMenu(menu);
                    action.Url = url;
                }
            }
        }

        private void ExtractWebconfigSection(List<string> lines, bool build) {

            for (; lines.Count > 0;) {
                // get the web config line
                string webcf = lines.First();
                if (webcf.StartsWith("::"))
                    break; // start of a new section

                ++_LineCounter; lines.RemoveAt(0);
                if (webcf.Trim() == "")
                    continue;
                if (string.IsNullOrWhiteSpace(webcf))
                    throw TemplateError("Web config section missing");
                if (!webcf.StartsWith("P:"))
                    throw TemplateError("Web config section must start with P: and can't be indented");

                if (build) {
                    string[] s = webcf.Split(new[] { '=' }, 2);
                    if (s.Length != 2) throw TemplateError("Invalid web config entry");
                    string key = s[0].Trim();
                    string value = s[1].Trim();
                    int len = value.Length;
                    if (len >= 2) {// trim quotes (if any)
                        if (value[0] == '"' && value[len - 1] == '"')
                            value = value.Substring(1, len - 2);
                    }
                    WebConfigHelper.SetValue(key, value);
                }
            }
        }

        private void ExtractCommandSection(List<string> lines, bool build) {
            for (; lines.Count > 0;) {
                // get the command line
                string cmd = lines.First();
                if (cmd.StartsWith("::"))
                    break; // start of a new section

                ++_LineCounter; lines.RemoveAt(0);
                if (cmd.Trim() == "")
                    continue;

                EvaluateVariable(cmd);
            }
        }
        private ModuleAction AddMenu(string menuText) {
            ModuleAction menuEntry = GetMenuAction(ref menuText);
            ModuleAction parentAction = FindAction(_SiteMenu, menuText);
            if (parentAction != null) {
                if (parentAction.SubMenu == null)
                    parentAction.SubMenu = new SerializableList<ModuleAction>() { menuEntry };
                else
                    parentAction.SubMenu.Add(menuEntry);
            } else
                _SiteMenu.Add(menuEntry);
            return menuEntry;
        }
        // Find an action with the specified menu text
        private ModuleAction FindAction(SerializableList<ModuleAction> menuList, string menuText) {
            if (string.IsNullOrWhiteSpace(menuText)) return null;
            string mtext = GetFirstMenuText(ref menuText);
            foreach (ModuleAction action in menuList) {
                if (action.MenuText == mtext) {
                    if (string.IsNullOrWhiteSpace(menuText))
                        return action;// found lowest menu matching the entire menu string menu>submenu>submenu
                    if (action.SubMenu == null || action.SubMenu.Count == 0) {
                        action.SubMenu = new SerializableList<ModuleAction> {
                            new ModuleAction() {
                                MenuText = mtext,
                                LinkText = mtext,
                                SubMenu = new SerializableList<ModuleAction>(),
                            }
                        };
                    }
                    return FindAction(action.SubMenu, menuText);
                }
            }
            ModuleAction newParent = new ModuleAction() {
                MenuText = mtext,
                LinkText = mtext,
                SubMenu = new SerializableList<ModuleAction>(),
            };
            menuList.Add(newParent);
            if (string.IsNullOrWhiteSpace(menuText))
                return newParent;
            else
                return FindAction(newParent.SubMenu, menuText);
        }
        // get the action text from the entire menu string menu>submenu>submenu>action
        private ModuleAction GetMenuAction(ref string menuText) {
            string[] s = menuText.Split(new char[] { '>' });
            string menuString;
            if (s.Length == 0) {
                menuString = menuText; // there is just the menu text
            } else {
                // return remaining menu text
                menuText = string.Join(">", s.Take(s.Length - 1));
                menuString = s[s.Length - 1];
            }
            menuString = menuString.Trim();
            object obj = EvaluateVariable(menuString);
            if (obj is string) {
                string text = (string)obj;
                ModuleAction action = new ModuleAction(null) {
                    Category = ModuleAction.ActionCategoryEnum.Read,
                    Location = ModuleAction.ActionLocationEnum.MainMenu,
                    LinkText = text,
                    MenuText = text,
                    Style = ModuleAction.ActionStyleEnum.Normal,
                };
                return action;
            } else if (obj is ModuleAction) {
                // create a copy with all attributes except queryargs as they can't be serialized, convert to Url instead
                ModuleAction action = new ModuleAction();
                ObjectSupport.CopyData(obj, action);
                action.Url = action.GetCompleteUrl();
                action.QueryArgs = null;
                action.QueryArgsHR = null;
                action.QueryArgsDict = null;
                return action;
            } else
                throw TemplateError("Unknown menu action {0}", menuString);
        }
        // get the first menu level from the entire menu string menu>submenu>submenu>action
        private string GetFirstMenuText(ref string menuText) {
            string[] s = menuText.Split(new char[] { '>' });
            menuText = string.Join(">", s.Skip(1));
            return s[0].Trim();
        }
        private void RemoveMenu(string menuText) {
            string[] menuStrings = menuText.Split(new char[] { '>' });
            SerializableList<ModuleAction> menu = _SiteMenu;
            for (int si = 0, maxSi = menuStrings.Count(); si < maxSi; ++si) {
                string menuString = menuStrings[si].Trim();
                object obj = EvaluateVariable(menuString);
                if (obj is ModuleAction) {
                    ModuleAction action = (ModuleAction)obj;
                    menuString = action.MenuText.ToString();
                } else
                    menuString = (string)obj;
                bool found = false;
                for (int mi = 0; mi < menu.Count(); ++mi) {
                    ModuleAction action = menu[mi];
                    if (action.MenuText.ToString() == menuString) {
                        if (si >= maxSi - 1) {
                            // last string entry, remove menu entry
                            if (action.SubMenu == null || action.SubMenu.Count() == 0) {
                                menu.RemoveAt(mi);
                                return;
                            }
                        } else {
                            if (action.SubMenu != null && action.SubMenu.Count() > 0) {
                                menu = action.SubMenu;
                                found = true;
                                break;
                            }
                        }
                    }
                }
                if (!found) return;
            }
        }
        private object EvaluateVariable(string expr) {
            // "string"     -> string
            // {asm,type}.method_call(parms)   ->  invoke method on object. All parms must be constants.
            expr = expr.Trim();
            string origExpr = expr;
            if (expr.StartsWith("{")) {
                // extract the asm,type to get the module
                expr = expr.Substring(1).Trim();
                int i = expr.IndexOf('}');// find ending }
                if (i < 0) throw TemplateError("Missing }} in expression \"{0}\"", origExpr);
                string asmtype = expr.Substring(0, i);
                expr = expr.Substring(i + 1).Trim();
                if (string.IsNullOrWhiteSpace(asmtype)) throw TemplateError("Missing assembly,type in expression \"{0}\"", origExpr);

                object obj;
                try {
                    obj = EvaluateObjectAsmTypeExpression(asmtype);
                } catch (Exception exc) {
                    throw TemplateError("Can't create object {0}: {1}", asmtype, exc.Message);
                }

                // get the method call
                if (!expr.StartsWith("."))
                    if (string.IsNullOrWhiteSpace(asmtype)) throw TemplateError("Missing method call in expression \"{0}\"", origExpr);
                expr = expr.Substring(1);

                return InvokeMethod(expr, origExpr, obj);
            } else {
                // return as string
                return expr;
            }
        }

        private object InvokeMethod(string expr, string origExpr, object obj) {
            int i = expr.IndexOf('(');
            if (i < 0) throw TemplateError("Invalid method call (open parenthesis required) in expression \"{0}\"", origExpr);
            string methodName = expr.Substring(0, i);
            if (string.IsNullOrWhiteSpace(methodName)) throw TemplateError("Missing method name in expression \"{0}\"", origExpr);
            expr = expr.Substring(i + 1);
            if (!expr.EndsWith(")"))
                throw TemplateError("Invalid method call (closing parenthesis required) in expression \"{0}\"", origExpr);
            expr = expr.Substring(0, expr.Length - 1);

            MethodInfo mi = obj.GetType().GetMethod(methodName);
            if (mi == null)
                throw TemplateError("Object {0} doesn't have a method call named {1} in expression \"{2}\"", obj.GetType().FullName, methodName, origExpr);

            List<object> parmList = new List<object>();
            if (!string.IsNullOrWhiteSpace(expr)) {
                string[] parms = expr.Split(new[] { ',' });
                foreach (string parm in parms) {
                    string p = parm.Trim();
                    if (p == "null")
                        parmList.Add(null);
                    else if (p == "0")
                        parmList.Add(0);
                    else if (p == "true")
                        parmList.Add(true);
                    else if (p == "false")
                        parmList.Add(false);
                    else if (p == "{SITEMENU}") // the entire site menu
                        parmList.Add(_SiteMenu);
                    else if (p == "{CURRENTPAGE}") // the current page
                        parmList.Add(_CurrentPage);
                    else if (p == "{CURRENTURL}") // the Url of the current page
                        parmList.Add(_CurrentUrl);
                    else if (p.StartsWith("\"")) // translate \r\n to newline
                        parmList.Add(TrimQuotes(p.Replace("\\r\\n", Environment.NewLine)));
                    else if (p.StartsWith("Guid(") && p.EndsWith(")"))  // Guid(nnnnn)
                        parmList.Add(new Guid(p.Substring(5, p.Length - 6)));
                    else {
                        int val;
                        float flt;
                        decimal dec;
                        if (p.EndsWith("f") && float.TryParse(p.Substring(0, p.Length - 1), out flt))
                            parmList.Add(flt);
                        else if (p.EndsWith("m") && decimal.TryParse(p.Substring(0, p.Length - 1), out dec))
                            parmList.Add(dec);
                        else if (int.TryParse(p, out val))
                            parmList.Add(val);
                        else
                            throw TemplateError("Invalid parameter {0} for method {1}", p, methodName);
                    }
                }
            }
            object result;
            try {
                result = mi.Invoke(obj, parmList.ToArray());
            } catch (Exception exc) {
                throw TemplateError("Can't call {0} in expression \"{1}\": {2}", methodName, origExpr, exc.Message);
            }
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private void ExtractSQLSection(List<string> lines, bool build) {

            string sqlConn = WebConfigHelper.GetValue<string>(DataProviderImpl.DefaultString, SQLBase.SQLConnectString);
            if (string.IsNullOrWhiteSpace(sqlConn)) throw TemplateError("No SQL connection string found in Appsettings.json (P:Default:SQLConnect)");

            for ( ; lines.Count > 0;) {

                // get the command line
                string sqlLine = lines.First();
                if (sqlLine.StartsWith("::"))
                    break; // start of a new section

                ++_LineCounter; lines.RemoveAt(0);
                if (sqlLine.Trim() == "")
                    continue;
                if (sqlLine.Trim() != "<<SQL")
                    throw TemplateError("Sql start (<<SQL) missing");
                string sqlLines = "";

                for (; lines.Count > 0;) {
                    sqlLine = lines.First();
                    ++_LineCounter; lines.RemoveAt(0);
                    if (sqlLine.Trim() == ">>SQL") {
                        if (build) {
                            using (SqlConnection conn = new SqlConnection(sqlConn)) {
                                conn.Open();
                                SqlCommand cmd = new SqlCommand(sqlLines, conn);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        continue;
                    }
                    if (sqlLine.Trim() != "GO")
                        sqlLines += sqlLine + "\r\n";
                }
            }
        }
        private void ExtractINCSection(List<string> lines, bool build, string section) {
            string[] parts = section.Split(new char[] { ' ' }, 2);
            if (parts.Length != 2)
                throw TemplateError("::INC statement invalid");
            string file = parts[1].Trim();
            file = TrimQuotes(file);
            BuildSite(file, build);
        }
    }
}
