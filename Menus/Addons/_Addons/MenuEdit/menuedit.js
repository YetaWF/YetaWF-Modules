/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

var YetaWF_MenuEdit = {};

YetaWF_MenuEdit.LoadTree = function (treeId, detailsId, data, newEntry) {
    'use strict';

    var JSTREE = 'jsTree';

    var $tree = $('#' + treeId + ' > .t_treeview');
    if ($tree.length != 1) throw 'Tree control not found';/*DEBUG*/
    var $details = $('#' + detailsId);
    if ($details.length != 1) throw 'Details control not found';/*DEBUG*/

    var MenuEntryType_Entry = "0";
    var MenuEntryType_Parent = "1";
    var MenuEntryType_Separator = "2";

    var currentNode = null;
    var currentNodeChanged = false;

    var treefuncs = {};
    treefuncs.GetData = function (tree) {
        function CvtjsTreeDataEntry(jsentry, entry) {
            entry.SubMenu = CvtjsTreeData(jsentry.children, []);
        }
        function CvtjsTreeData(jsdata, data) {
            if (jsdata == undefined) return undefined;
            jsdata.forEach(function (jsentry) {
                var d = tree.jstree('get_node', jsentry.id);
                data.push(d.original);
                CvtjsTreeDataEntry(jsentry, d.original);
            });
            return data;
        }
        var jsdata = tree.jstree('get_json');
        var data = [];
        data = CvtjsTreeData(jsdata, data);
        return data;
    };
    treefuncs.GetSelectedNode = function (tree) {
        var $node = tree.jstree('get_selected', true);
        if ($node.length == 1) return $node[0];
        return null;
    };
    treefuncs.SetSelectedNode = function (tree, node) {
        tree.jstree('deselect_all', true); // don't trigger change event
        tree.jstree('select_node', node.id, false, false);// don't trigger change event
    };
    treefuncs.SelectRootNode = function (tree) {
        tree.jstree('deselect_all', true); // don't trigger change event
        tree.jstree('select_node', 'ul li:first', false, false);// trigger event change
    };
    treefuncs.GetDataItem = function (tree, node) {
        return node.original;
    };
    treefuncs.IsRootNode = function (tree, node) {
        if (node == null) return true;
        return node.id == "#" || node.parent == "#";
    };
    treefuncs.ExpandAll = function (tree) {
        tree.jstree('open_all');
    };
    treefuncs.CollapseAll = function (tree) {
        tree.jstree('close_all');
    };
    treefuncs.HaveMoreThanRootNode = function (tree) {
        var root = tree.jstree('get_node', '#');
        if (root == undefined) return false;
        return !tree.jstree('is_leaf', root);
    };
    treefuncs.AddNode = function (tree, node) {
        var node = tree.jstree('create_node', node, newEntry, 'first', function () { }, true);
        return tree.jstree('get_node', node);
    };
    treefuncs.ValidNode = function (tree, node) {
        return (node != null);
    };
    treefuncs.GetNextNode = function (tree, node) {
        var node = tree.jstree('get_next_dom', node, false);
        node = tree.jstree('get_node', node)
        if (node == false) {
            node = tree.jstree('get_prev_dom', node, false);
            node = tree.jstree('get_node', node)
        }
        return node ? node : null;
    };
    treefuncs.DeleteNode = function (tree, node) {
        tree.jstree('delete_node', node);
    };
    treefuncs.UpdateText = function (tree, node, text) {
        tree.jstree('rename_node', node, text);
    }

    // field handling
    function EnableFields(enable) {
        if (enable) {
            YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.EntryType']", $details), true);
            var entry = $("select[name='ModAction.EntryType']", $details).val();
            switch (entry) {
                default:
                    throw "Invalid entry type {0}".format(entry);/*DEBUG*/
                case MenuEntryType_Entry:
                    YetaWF_Url.Enable($("div[data-name='ModAction.Url']", $details), true);
                    YetaWF_ModuleSelection.Enable($("div[data-name='ModAction.SubModule']", $details), true);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.MenuText']", $details), true);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.LinkText']", $details), true);
                    $("input[name='ModAction.ImageUrlFinal']", $details).removeAttr('disabled');
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.Tooltip']", $details), true);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.Legend']", $details), true);
                    $("input:checkbox[name='ModAction.Enabled']", $details).removeAttr('disabled');
                    $("input[name='ModAction.CssClass']", $details).removeAttr('disabled');
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Style']", $details), true);
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Mode']", $details), true);
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Category']", $details), true);
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.LimitToRole']", $details), true);
                    $("input:checkbox[name='ModAction.AuthorizationIgnore']", $details).removeAttr('disabled');
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.ConfirmationText']", $details), true);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.PleaseWaitText']", $details), true);
                    $("input:checkbox[name='ModAction.SaveReturnUrl']", $details).removeAttr('disabled');
                    $("input:checkbox[name='ModAction.AddToOriginList']", $details).removeAttr('disabled');
                    $("input:checkbox[name='ModAction.NeedsModuleContext']", $details).removeAttr('disabled');
                    $("input:checkbox[name='ModAction.DontFollow']", $details).removeAttr('disabled');
                    break;
                case MenuEntryType_Parent:
                    YetaWF_Url.Enable($("div[data-name='ModAction.Url']", $details), false);
                    YetaWF_ModuleSelection.Enable($("div[data-name='ModAction.SubModule']", $details), false);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.MenuText']", $details), true);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.LinkText']", $details), true);
                    $("input[name='ModAction.ImageUrlFinal']", $details).removeAttr('disabled');
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.Tooltip']", $details), true);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.Legend']", $details), true);
                    $("input:checkbox[name='ModAction.Enabled']", $details).removeAttr('disabled');
                    $("input[name='ModAction.CssClass']", $details).removeAttr('disabled');
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Style']", $details), true);
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Mode']", $details), true);
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Category']", $details), true);
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.LimitToRole']", $details), true);
                    $("input:checkbox[name='ModAction.AuthorizationIgnore']", $details).attr('disabled', 'disabled');
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.ConfirmationText']", $details), false);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.PleaseWaitText']", $details), false);
                    $("input:checkbox[name='ModAction.SaveReturnUrl']", $details).attr('disabled', 'disabled');
                    $("input:checkbox[name='ModAction.AddToOriginList']", $details).attr('disabled', 'disabled');
                    $("input:checkbox[name='ModAction.NeedsModuleContext']", $details).attr('disabled', 'disabled');
                    $("input:checkbox[name='ModAction.DontFollow']", $details).attr('disabled', 'disabled');
                    break;
                case MenuEntryType_Separator:
                    YetaWF_Url.Enable($("div[data-name='ModAction.Url']", $details), false);
                    YetaWF_ModuleSelection.Enable($("div[data-name='ModAction.SubModule']", $details), false);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.MenuText']", $details), false);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.LinkText']", $details), false);
                    $("input[name='ModAction.ImageUrlFinal']", $details).attr('disabled', 'disabled');
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.Tooltip']", $details), false);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.Legend']", $details), false);
                    $("input:checkbox[name='ModAction.Enabled']", $details).attr('disabled', 'disabled');
                    $("input[name='ModAction.CssClass']", $details).attr('disabled', 'disabled');
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Style']", $details), false);
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Mode']", $details), false);
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Category']", $details), false);
                    YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.LimitToRole']", $details), false);
                    $("input:checkbox[name='ModAction.AuthorizationIgnore']", $details).attr('disabled', 'disabled');
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.ConfirmationText']", $details), false);
                    YetaWF_MultiString.Enable($("div[data-name='ModAction.PleaseWaitText']", $details), false);
                    $("input:checkbox[name='ModAction.SaveReturnUrl']", $details).attr('disabled', 'disabled');
                    $("input:checkbox[name='ModAction.AddToOriginList']", $details).attr('disabled', 'disabled');
                    $("input:checkbox[name='ModAction.NeedsModuleContext']", $details).attr('disabled', 'disabled');
                    $("input:checkbox[name='ModAction.DontFollow']", $details).attr('disabled', 'disabled');
                    break;
            }
        } else {
            YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.EntryType']", $details), false);
            YetaWF_Url.Enable($("div[data-name='ModAction.Url']", $details), false);
            YetaWF_ModuleSelection.Enable($("div[data-name='ModAction.SubModule']", $details), false);
            YetaWF_MultiString.Enable($("div[data-name='ModAction.MenuText']", $details), false);
            YetaWF_MultiString.Enable($("div[data-name='ModAction.LinkText']", $details), false);
            $("input[name='ModAction.ImageUrlFinal']", $details).attr('disabled', 'disabled');
            YetaWF_MultiString.Enable($("div[data-name='ModAction.Tooltip']", $details), false);
            YetaWF_MultiString.Enable($("div[data-name='ModAction.Legend']", $details), false);
            $("input:checkbox[name='ModAction.Enabled']", $details).attr('disabled', 'disabled');
            $("input[name='ModAction.CssClass']", $details).attr('disabled', 'disabled');
            YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Style']", $details), false);
            YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Mode']", $details), false);
            YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.Category']", $details), false);
            YetaWF_TemplateDropDownList.Enable($("select[name='ModAction.LimitToRole']", $details), false);
            $("input:checkbox[name='ModAction.AuthorizationIgnore']", $details).attr('disabled', 'disabled');
            YetaWF_MultiString.Enable($("div[data-name='ModAction.ConfirmationText']", $details), false);
            YetaWF_MultiString.Enable($("div[data-name='ModAction.PleaseWaitText']", $details), false);
            $("input:checkbox[name='ModAction.SaveReturnUrl']", $details).attr('disabled', 'disabled');
            $("input:checkbox[name='ModAction.AddToOriginList']", $details).attr('disabled', 'disabled');
            $("input:checkbox[name='ModAction.NeedsModuleContext']", $details).attr('disabled', 'disabled');
            $("input:checkbox[name='ModAction.DontFollow']", $details).attr('disabled', 'disabled');
        }
    }
    function UpdateFields(dataItem) {
        YetaWF_TemplateDropDownList.Update($("select[name='ModAction.EntryType']", $details), dataItem.EntryType);
        $("input[name='ModAction._Text']", $details).val(dataItem._Text);
        YetaWF_Url.Update($("div[data-name='ModAction.Url']", $details), dataItem.Url, true);
        YetaWF_ModuleSelection.UpdateComplete($("div[data-name='ModAction.SubModule']", $details), dataItem.SubModule);
        YetaWF_MultiString.Update($("div[data-name='ModAction.MenuText']", $details), dataItem.MenuText);
        YetaWF_MultiString.Update($("div[data-name='ModAction.LinkText']", $details), dataItem.LinkText);
        $("input[name='ModAction.ImageUrlFinal']", $details).val(dataItem.ImageUrlFinal);
        YetaWF_MultiString.Update($("div[data-name='ModAction.Tooltip']", $details), dataItem.Tooltip);
        YetaWF_MultiString.Update($("div[data-name='ModAction.Legend']", $details), dataItem.Legend);
        $("input:checkbox[name='ModAction.Enabled']", $details).prop('checked', dataItem.Enabled);
        $("input[name='ModAction.CssClass']", $details).val(dataItem.CssClass);
        YetaWF_TemplateDropDownList.Update($("select[name='ModAction.Style']", $details), dataItem.Style);
        YetaWF_TemplateDropDownList.Update($("select[name='ModAction.Mode']", $details), dataItem.Mode);
        YetaWF_TemplateDropDownList.Update($("select[name='ModAction.Category']", $details), dataItem.Category);
        YetaWF_TemplateDropDownList.Update($("select[name='ModAction.LimitToRole']", $details), dataItem.LimitToRole);
        $("input:checkbox[name='ModAction.AuthorizationIgnore']", $details).prop('checked', dataItem.AuthorizationIgnore);
        YetaWF_MultiString.Update($("div[data-name='ModAction.ConfirmationText']", $details), dataItem.ConfirmationText);
        YetaWF_MultiString.Update($("div[data-name='ModAction.PleaseWaitText']", $details), dataItem.PleaseWaitText);
        $("input:checkbox[name='ModAction.SaveReturnUrl']", $details).prop('checked', dataItem.SaveReturnUrl);
        $("input:checkbox[name='ModAction.AddToOriginList']", $details).prop('checked', dataItem.AddToOriginList);
        $("input:checkbox[name='ModAction.NeedsModuleContext']", $details).prop('checked', dataItem.NeedsModuleContext);
        $("input:checkbox[name='ModAction.DontFollow']", $details).prop('checked', dataItem.DontFollow);
    }
    function ClearFields(dataItem) {
        $("input[name='ActiveEntry']", $details).val(0);
        $("input[name='NewAfter']", $details).val(0);

        YetaWF_TemplateDropDownList.Update($("select[name='ModAction.EntryType']", $details), MenuEntryType_Entry);
        $("input[name='ModAction._Text']", $details).val('');
        YetaWF_Url.Clear($("div[data-name='ModAction.Url']", $details));
        YetaWF_ModuleSelection.Clear($("div[data-name='ModAction.SubModule']", $details));
        YetaWF_MultiString.Clear($("div[data-name='ModAction.MenuText']", $details));
        YetaWF_MultiString.Clear($("div[data-name='ModAction.LinkText']", $details));
        $("input[name='ModAction.ImageUrlFinal']", $details).val('');
        YetaWF_MultiString.Clear($("div[data-name='ModAction.Tooltip']", $details));
        YetaWF_MultiString.Clear($("div[data-name='ModAction.Legend']", $details));
        $("input:checkbox[name='ModAction.Enabled']", $details).prop('checked', false);
        $("input[name='ModAction.CssClass']", $details).val('');
        YetaWF_TemplateDropDownList.Clear($("select[name='ModAction.Style']", $details));
        YetaWF_TemplateDropDownList.Clear($("select[name='ModAction.Mode']", $details));
        YetaWF_TemplateDropDownList.Clear($("select[name='ModAction.Category']", $details));
        YetaWF_TemplateDropDownList.Clear($("select[name='ModAction.LimitToRole']", $details));
        $("input:checkbox[name='ModAction.AuthorizationIgnore']", $details).prop('checked', false);
        YetaWF_MultiString.Clear($("div[data-name='ModAction.ConfirmationText']", $details));
        YetaWF_MultiString.Clear($("div[data-name='ModAction.PleaseWaitText']", $details));
        $("input:checkbox[name='ModAction.SaveReturnUrl']", $details).prop('checked', false);
        $("input:checkbox[name='ModAction.AddToOriginList']", $details).prop('checked', false);
        $("input:checkbox[name='ModAction.NeedsModuleContext']", $details).prop('checked', false);
        $("input:checkbox[name='ModAction.DontFollow']", $details).prop('checked', false);
    }
    function HasChanged(dataItem) {

        if (currentNodeChanged) return true;
        var val;
        var entry = $("select[name='ModAction.EntryType']", $details).val();
        if (dataItem.EntryType != entry) return true;

        if (YetaWF_Url.HasChanged($("div[data-name='ModAction.Url']", $details), dataItem.Url)) return true;
        if (YetaWF_ModuleSelection.HasChanged($("div[data-name='ModAction.SubModule']", $details), dataItem.SubModule)) return true;
        if (YetaWF_MultiString.HasChanged($("div[data-name='ModAction.MenuText']", $details), dataItem.MenuText)) return true;
        if (YetaWF_MultiString.HasChanged($("div[data-name='ModAction.LinkText']", $details), dataItem.LinkText)) return true;
        val = $("input[name='ModAction.ImageUrlFinal']", $details).val();
        if (!StringYCompare(dataItem.ImageUrlFinal, val)) return true;
        if (YetaWF_MultiString.HasChanged($("div[data-name='ModAction.Tooltip']", $details), dataItem.Tooltip)) return true;
        if (YetaWF_MultiString.HasChanged($("div[data-name='ModAction.Legend']", $details), dataItem.Legend)) return true;
        val = $("input:checkbox[name='ModAction.Enabled']", $details).prop('checked');
        if (dataItem.Enabled != val) return true;
        val = $("input[name='ModAction.CssClass']", $details).val();
        if (!StringYCompare(dataItem.CssClass, val)) return true;
        val = $("select[name='ModAction.Style']", $details).val();
        if (dataItem.Style != val) return true;
        val = $("select[name='ModAction.Mode']", $details).val();
        if (dataItem.Mode != val) return true;
        val = $("select[name='ModAction.Category']", $details).val();
        if (dataItem.Category != val) return true;
        val = $("select[name='ModAction.LimitToRole']", $details).val();
        if (dataItem.LimitToRole != val) return true;
        val = $("input:checkbox[name='ModAction.AuthorizationIgnore']", $details).prop('checked');
        if (dataItem.AuthorizationIgnore != val) return true;
        if (YetaWF_MultiString.HasChanged($("div[data-name='ModAction.ConfirmationText']", $details), dataItem.ConfirmationText)) return true;
        if (YetaWF_MultiString.HasChanged($("div[data-name='ModAction.PleaseWaitText']", $details), dataItem.PleaseWaitText)) return true;
        val = $("input:checkbox[name='ModAction.SaveReturnUrl']", $details).prop('checked');
        if (dataItem.SaveReturnUrl != val) return true;
        val = $("input:checkbox[name='ModAction.AddToOriginList']", $details).prop('checked');
        if (dataItem.AddToOriginList != val) return true;
        val = $("input:checkbox[name='ModAction.NeedsModuleContext']", $details).prop('checked');
        if (dataItem.NeedsModuleContext != val) return true;
        val = $("input:checkbox[name='ModAction.DontFollow']", $details).prop('checked');
        if (dataItem.DontFollow != val) return true;

        return false;
    }
    function SaveFields(dataItem) {

        var entry = $("select[name='ModAction.EntryType']", $details).val();
        dataItem.EntryType = entry;

        switch (entry) {
            default:
                throw "Invalid entry type {0}".format(entry);/*DEBUG*/
            case MenuEntryType_Entry:
                dataItem.Separator = false;
                break;
            case MenuEntryType_Parent:
                dataItem.Separator = false;
                YetaWF_Url.Clear($("div[data-name='ModAction.Url']", $details));
                YetaWF_ModuleSelection.Clear($("div[data-name='ModAction.SubModule']", $details));
                $("select[name='ModAction.Style']", $details).prop('selectedIndex', 0);
                $("select[name='ModAction.Mode']", $details).prop('selectedIndex', 0);
                $("select[name='ModAction.Category']", $details).prop('selectedIndex', 0);
                YetaWF_MultiString.Clear($("div[data-name='ModAction.ConfirmationText']", $details));
                YetaWF_MultiString.Clear($("div[data-name='ModAction.PleaseWaitText']", $details));
                $("input:checkbox[name='ModAction.SaveReturnUrl']", $details).prop('checked', false);
                $("input:checkbox[name='ModAction.AddToOriginList']", $details).prop('checked', false);
                $("input:checkbox[name='ModAction.NeedsModuleContext']", $details).prop('checked', false);
                $("input:checkbox[name='ModAction.DontFollow']", $details).prop('checked', false);
                break;
            case MenuEntryType_Separator:
                dataItem.Separator = true;
                YetaWF_Url.Clear($("div[data-name='ModAction.Url']", $details));
                YetaWF_ModuleSelection.Clear($("div[data-name='ModAction.SubModule']", $details));
                YetaWF_MultiString.Clear($("div[data-name='ModAction.MenuText']", $details));
                YetaWF_MultiString.Clear($("div[data-name='ModAction.LinkText']", $details));
                $("input[name='ModAction.ImageUrlFinal']", $details).val('');
                YetaWF_MultiString.Clear($("div[data-name='ModAction.Tooltip']", $details));
                YetaWF_MultiString.Clear($("div[data-name='ModAction.Legend']", $details));
                $("input:checkbox[name='ModAction.Enabled']", $details).prop('checked', false);
                $("input[name='ModAction.CssClass']", $details).val('');
                $("select[name='ModAction.Style']", $details).prop('selectedIndex', 0);
                $("select[name='ModAction.Mode']", $details).prop('selectedIndex', 0);
                $("select[name='ModAction.Category']", $details).prop('selectedIndex', 0);
                $("select[name='ModAction.LimitToRole']", $details).prop('selectedIndex', 0);
                $("input:checkbox[name='ModAction.AuthorizationIgnore']", $details).prop('checked', false);
                YetaWF_MultiString.Clear($("div[data-name='ModAction.ConfirmationText']", $details));
                YetaWF_MultiString.Clear($("div[data-name='ModAction.PleaseWaitText']", $details));
                $("input:checkbox[name='ModAction.SaveReturnUrl']", $details).prop('checked', false);
                $("input:checkbox[name='ModAction.AddToOriginList']", $details).prop('checked', false);
                $("input:checkbox[name='ModAction.NeedsModuleContext']", $details).prop('checked', false);
                $("input:checkbox[name='ModAction.DontFollow']", $details).prop('checked', false);
                break;
        }

        // Save fields in tree when they change
        dataItem.Url = YetaWF_Url.Retrieve($("div[data-name='ModAction.Url']", $details));
        dataItem.SubModule = YetaWF_ModuleSelection.Retrieve($("div[data-name='ModAction.SubModule']", $details));
        YetaWF_MultiString.Retrieve($("div[data-name='ModAction.MenuText']", $details), dataItem.MenuText);
        YetaWF_MultiString.Retrieve($("div[data-name='ModAction.LinkText']", $details), dataItem.LinkText);
        dataItem.ImageUrlFinal = $("input[name='ModAction.ImageUrlFinal']", $details).val();
        YetaWF_MultiString.Retrieve($("div[data-name='ModAction.Tooltip']", $details), dataItem.Tooltip);
        YetaWF_MultiString.Retrieve($("div[data-name='ModAction.Legend']", $details), dataItem.Legend);
        dataItem.Enabled = $("input:checkbox[name='ModAction.Enabled']", $details).prop('checked');
        dataItem.CssClass = $("input[name='ModAction.CssClass']", $details).val();
        dataItem.Style = $("select[name='ModAction.Style']", $details).val();
        dataItem.Mode = $("select[name='ModAction.Mode']", $details).val();
        dataItem.Category = $("select[name='ModAction.Category']", $details).val();
        dataItem.LimitToRole = $("select[name='ModAction.LimitToRole']", $details).val();
        dataItem.AuthorizationIgnore = $("input:checkbox[name='ModAction.AuthorizationIgnore']", $details).prop('checked');
        YetaWF_MultiString.Retrieve($("div[data-name='ModAction.ConfirmationText']", $details), dataItem.ConfirmationText);
        YetaWF_MultiString.Retrieve($("div[data-name='ModAction.PleaseWaitText']", $details), dataItem.PleaseWaitText);
        dataItem.SaveReturnUrl = $("input:checkbox[name='ModAction.SaveReturnUrl']", $details).prop('checked');
        dataItem.AddToOriginList = $("input:checkbox[name='ModAction.AddToOriginList']", $details).prop('checked');
        dataItem.NeedsModuleContext = $("input:checkbox[name='ModAction.NeedsModuleContext']", $details).prop('checked');
        dataItem.DontFollow = $("input:checkbox[name='ModAction.DontFollow']", $details).prop('checked');

        // update tree control
        switch (entry) {
            default:
                throw "Invalid entry type {0}".format(entry);/*DEBUG*/
            case MenuEntryType_Entry:
            case MenuEntryType_Parent:
                dataItem.text = dataItem._Text = YetaWF_MultiString.getDefaultValue($("div[data-name='ModAction.MenuText']"));
                break;
            case MenuEntryType_Separator:
                dataItem.text = dataItem._Text = YLocs.YetaWF_Menus.Separator;
                break;
        }
    }
    function UpdateButtons(tree, node) {
        // save, reset, delete
        if (treefuncs.ValidNode(tree, node) && !treefuncs.IsRootNode(tree, node)) {
            $("input[name='t_submit']", $details).button("enable");
            $("input[name='t_delete']", $details).button("enable");
            var dataItem = treefuncs.GetDataItem(tree, node);
            $("input[name='t_reset']", $details).button("enable");
        } else {
            $("input[name='t_submit']", $details).button("disable");
            $("input[name='t_reset']", $details).button("disable");
            $("input[name='t_delete']", $details).button("disable");
        }
        // add new
        if (treefuncs.ValidNode(tree, node))
            $("input[name='t_add']", $details).button("enable");
        else
            $("input[name='t_add']", $details).button("disable");
        if (treefuncs.HaveMoreThanRootNode(tree)) {
            $("input[name='t_expandall']", $details).button("enable");
            $("input[name='t_collapseall']", $details).button("enable");
        } else {
            $("input[name='t_expandall']", $details).button("disable");
            $("input[name='t_collapseall']", $details).button("disable");
        }
    }
    // enable/disable fields based on current node
    function CondEnable(tree, node)
    {
        if (!treefuncs.ValidNode(tree, node) || treefuncs.IsRootNode(tree, node)) {
            // root node
            ClearFields();
            EnableFields(false);
        } else {
            // new node
            var dataItem = treefuncs.GetDataItem(tree, node);
            UpdateFields(dataItem);
            EnableFields(true);
        }
        UpdateButtons(tree, node);
    }

    // update active entry
    function UpdateCurrentEntry(current) {
        $("input[name='ActiveEntry']", $details).val(current);
        $("input[name='NewAfter']", $details).val(0);
    }
    function UpdateNewEntry(newAfter) {
        $("input[name='ActiveEntry']", $details).val(0);
        $("input[name='NewAfter']", $details).val(newAfter);
    }
    function UpdateCurrentEntryByNode(tree, node) {
        if (treefuncs.IsRootNode(tree, node)) {
            UpdateCurrentEntry(0);
        } else {
            // new node
            var dataItem = treefuncs.GetDataItem(tree, node);
            UpdateCurrentEntry(dataItem.Id);
        }
    }
    function UpdateNewEntryByNode(tree, node) {
        if (treefuncs.IsRootNode(tree, node)) {
            UpdateNewEntry(0, 0);
        } else {
            // new node
            var dataItem = treefuncs.GetDataItem(tree, node);
            UpdateNewEntry(dataItem.Id);
        }
    }
    function PrepareNewEntry(tree, e, okFunc)
    {
        // save all current input fields in tree
        if (treefuncs.ValidNode(tree, currentNode) && !treefuncs.IsRootNode(tree, currentNode)) {
            // current node is not the root node
            var dataItem = treefuncs.GetDataItem(tree, currentNode);
            if (HasChanged(dataItem)) {
                if (e) e.preventDefault();// don't continue with selection change
                YetaWF_Basics.Y_Alert(YLocs.YetaWF_Menus.ChangedEntry);
                return false;
            }
        }
        okFunc();
        return true;
    }

    function SendEntireMenu(tree)
    {
        var $form = YetaWF_Forms.getForm($details);

        var ajaxurl = $details.attr('data-ajaxurl');
        if (ajaxurl == undefined) throw "Can't locate ajax url to validate and add user name";/*DEBUG*/

        var menuVersion = $('input[name="MenuVersion"]', $form).val();
        if (menuVersion == undefined || menuVersion.Length == 0) throw "No menu version found";/*DEBUG*/
        var menuGuid = $('input[name="MenuGuid"]', $form).val();
        if (menuGuid == undefined || menuGuid.Length == 0) throw "No menu guid found";/*DEBUG*/
        var postData = "EntireMenu=" + encodeURIComponent(JSON.stringify(treefuncs.GetData(tree)))
                       + "&menuGuid=" + encodeURIComponent(menuGuid)
                       + "&menuVersion=" + encodeURIComponent(menuVersion)
                       + YetaWF_Forms.getFormInfo($form).QS;
        $.ajax({
            url: ajaxurl,
            data: postData, cache: false, type: 'POST',
            dataType: 'html',
            success: function (result, textStatus, jqXHR) {
                if (result.startsWith(YConfigs.Basics.AjaxJavascriptReturn)) {
                    var script = result.substring(YConfigs.Basics.AjaxJavascriptReturn.length);
                    eval(script);
                    return;
                } else if (result.startsWith(YConfigs.Basics.AjaxJavascriptErrorReturn)) {
                    var script = result.substring(YConfigs.Basics.AjaxJavascriptErrorReturn.length);
                    eval(script);
                    return;
                }
                //server returns a new menu version
                var newVersion = JSON.parse(result);
                $('input[name="MenuVersion"]', $form).val(newVersion);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                YetaWF_Basics.Y_Alert(YLocs.Forms.AjaxError.format(jqXHR.status, jqXHR.statusText), YLocs.Forms.AjaxErrorTitle);
            }
        });
    }

    function SendOneEntry(tree, node) {

        var $form = YetaWF_Forms.getForm($details);
        var useValidation = !treefuncs.IsRootNode(tree, node);
        var extraData = "ValidateCurrent=" + JSON.stringify(useValidation);
        YetaWF_Forms.submit($form, useValidation, extraData,
            function (hasErrors) {
                if (!useValidation || !hasErrors) {
                    var dataItem = treefuncs.GetDataItem(tree, node);
                    if (!treefuncs.IsRootNode(tree, node))
                        SaveFields(dataItem);
                    treefuncs.UpdateText(tree, node, dataItem._Text);
                    CondEnable(tree, node);
                    currentNode = node;
                    currentNodeChanged = false;
                }
            });
    }

    var tree; // the tree object

    function MakejsTreeDataEntry(entry) {
        entry.text = entry._Text;
        entry.icon = false;
        entry.state = { opened: true, disabled: false, selected: false };
        entry.children = MakejsTreeData(entry.SubMenu);
        entry.SubMenu = null;
        if (entry.SubModule == null)
            entry.SubModule = "00000000-0000-0000-0000-000000000000";
    }
    function MakejsTreeData(data) {
        if (data == undefined) return undefined;
        data.forEach(function (entry) {
            MakejsTreeDataEntry(entry);
        });
        return data;
    }

    // Initialize the tree
    data = MakejsTreeData(data);
    MakejsTreeDataEntry(newEntry);

    $tree.jstree({
        multiple: false,
        plugins: ['dnd'],
        dnd: {
            copy: false,
            is_draggable: function (nodes, e) {
                if (treefuncs.IsRootNode(tree, nodes[0])) return false;
                return true;
            },
            drag_selection: false,
        },
        core: {
            'data': data,
            themes: {
                icons: false,
                dots: false,
            },
            check_callback: function (operation, node, node_parent, node_position, more) {
                console.log(operation + ' ' + node_parent.id + ' ' + node.id + ' ' + node_position);
                if (operation == 'move_node') {
                    if (node_parent.id == "#")
                        return false;
                }
                return true;
            },
        },
    });
    tree = $tree;
    $tree.css('overflow', 'auto');

    // selection change
    $tree.on('changed.jstree', function (e, data) {
        var node = data.node;
        if (currentNode != null && node == currentNode) return true;
        if (PrepareNewEntry(tree, e, function () {
                    CondEnable(tree, node);
                    UpdateCurrentEntryByNode(tree, node);
                    currentNode = node;
                    currentNodeChanged = false;
            })) {
            return true;
        } else {
            treefuncs.SetSelectedNode(tree, currentNode);
        }
    });
    //d&d drop
    $tree.on('move_node.jstree', function (e, data) {
        var node = data.node;
        CondEnable(tree, node);
        UpdateCurrentEntryByNode(tree, node);
        currentNode = node;
        SendEntireMenu(tree);
    });

    currentNode = null;
    currentNodeChanged = false;

    $(document).ready(function () {

        CondEnable(tree, currentNode);

        // when a separator is selected, enable/disable other fields
        $($details).on("change", "select[name='ModAction.EntryType']", function () {
            EnableFields(true);
            UpdateButtons(tree, treefuncs.GetSelectedNode(tree))
        });

        // save button
        $($details).on("click", "input[name='t_submit']", function (e) {
            e.preventDefault();
            var node = treefuncs.GetSelectedNode(tree);
            SendOneEntry(tree, node);
            return false;
        });
        // Reset button
        $("input[name='t_reset']", $details).on("click", function () {
            var node = treefuncs.GetSelectedNode(tree);
            if (!treefuncs.ValidNode(tree, node))
                YetaWF_Basics.Y_Alert(YLocs.YetaWF_Menus.NoMenuEntry);
            else if (treefuncs.IsRootNode(tree, node))
                YetaWF_Basics.Y_Alert(YLocs.YetaWF_Menus.NoResetMenu);
            else {
                CondEnable(tree, node);
                currentNode = node;
                currentNodeChanged = false;
            }
        });
        // Add New button
        $("input[name='t_add']", $details).on("click", function () {
            var node = treefuncs.GetSelectedNode(tree);
            if (!treefuncs.ValidNode(tree, node)) {
                YetaWF_Basics.Y_Alert(YLocs.YetaWF_Menus.NoMenuEntry);
                return;
            }
            PrepareNewEntry(tree, null, function () {
                var newNode = treefuncs.AddNode(tree, node);
                treefuncs.SetSelectedNode(tree, newNode);
                CondEnable(tree, newNode);
                UpdateNewEntryByNode(tree, node);
                currentNode = newNode;
                currentNodeChanged = true;
            });
        });
        // Delete button
        $("input[name='t_delete']", $details).on("click", function () {
            var node = treefuncs.GetSelectedNode(tree);
            if (!treefuncs.ValidNode(tree, node))
                YetaWF_Basics.Y_Alert(YLocs.YetaWF_Menus.NoMenuEntry);
            else if (treefuncs.IsRootNode(tree, node))
                YetaWF_Basics.Y_Alert(YLocs.YetaWF_Menus.NoRemoveMenu);
            else {
                var nextNode = treefuncs.GetNextNode(tree, node);
                treefuncs.DeleteNode(tree, node);
                if (treefuncs.ValidNode(tree, nextNode))
                    treefuncs.SetSelectedNode(tree, nextNode);
                SendEntireMenu(tree);
                node = treefuncs.GetSelectedNode(tree);
                CondEnable(tree, node);
                UpdateCurrentEntryByNode(tree, node);
                currentNode = node;
                currentNodeChanged = false;
            }
        });
        // expand button
        $("input[name='t_expandall']", $details).on("click", function () {
            treefuncs.ExpandAll(tree);
        });
        // collapse button
        $("input[name='t_collapseall']", $details).on("click", function () {
            PrepareNewEntry(tree, null, function () {
                treefuncs.CollapseAll(tree);
                treefuncs.SelectRootNode(tree);
            });
        });
    });
};
