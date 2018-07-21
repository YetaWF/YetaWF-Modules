/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
'use strict';

var YetaWF_MultiString = {};

if (YLocs.YetaWF_ComponentsHTML.Languages == undefined) throw "YLocs.YetaWF_ComponentsHTML.Languages missing";/*DEBUG*/

// Load a multistring UI object with data
// ms refers to the div class="yt_multistring t_edit"
// data is an object with language data for each language
// if no data is available for a language, the default is used
// this is mainly so we can always validate the input field without having to worry about the selected language
YetaWF_MultiString.Update = function($ms, data)
{
    var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
    for (var index = 0; index < count ; ++index) {
        var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
        var s = '';
        if (data.hasOwnProperty(lang))
            s = data[lang];
        else if (data.hasOwnProperty(YLocs.YetaWF_ComponentsHTML.Languages[0])) {
            s = data[lang] = data[YLocs.YetaWF_ComponentsHTML.Languages[0]];// use default for languages w/o data
        }
        else throw "No language data";/*DEBUG*/
        _YetaWF_MultiString.getHidden($ms, index).val(s);
        if (index == 0)
            _YetaWF_MultiString.getText($ms).val(s);
    }
    YetaWF_TemplateDropDownList.Clear(_YetaWF_MultiString.getDropDown($ms));
}

// Get data from multistring
// ms refers to the div class="yt_multistring t_edit"
// data is an object with language data for each language
YetaWF_MultiString.Retrieve = function ($ms, data) {

    // update the hidden field with the next textbox value
    var newText = _YetaWF_MultiString.getText($ms).val();
    var sel = _YetaWF_MultiString.getSelLanguage($ms);
    var $hidval = _YetaWF_MultiString.getHidden($ms, sel);
    $hidval.val(newText);

    // now check whether it actually changed
    // if nothing is specified for a language, save what is entered in the text box
    var changed = false;
    var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
    for (var index = 0; index < count ; ++index) {
        var langText = _YetaWF_MultiString.getHidden($ms, index).val();
        langText = langText.trim();
        if (langText == '')
            langText = newText;
        var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
        if (!$YetaWF.stringCompare(data[lang], langText)) {
            changed = true;
            data[lang] = langText;
        }
    }
    return changed;
}

// Test whether the data has changed
// ms refers to the div class="yt_multistring t_edit"
// data is an object with language data for each language
YetaWF_MultiString.HasChanged = function ($ms, data) {

    var text = _YetaWF_MultiString.getText($ms).val();

    // now check whether it actually changed
    var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
    for (var index = 0; index < count ; ++index) {
        var langText = _YetaWF_MultiString.getHidden($ms, index).val();
        if (langText.trim() == '')
            langText = text;
        var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
        if (data[lang] != null && !$YetaWF.stringCompare(data[lang], langText))
            return true;
    }
    return false;
}

// Enable a multistring object
// ms refers to the <div class="yt_multistring t_edit" data-name="...">
YetaWF_MultiString.Enable = function($ms, enabled)
{
    if (enabled) {
        _YetaWF_MultiString.getText($ms).removeAttr('disabled');
        if (YConfigs.YetaWF_ComponentsHTML.Localization)
            YetaWF_TemplateDropDownList.Enable(_YetaWF_MultiString.getDropDown($ms), true);
    } else {
        _YetaWF_MultiString.getText($ms).attr('disabled', 'disabled');
        YetaWF_TemplateDropDownList.Enable(_YetaWF_MultiString.getDropDown($ms), false);
    }
}

// Clear a multistring object
// ms refers to the <div class="yt_multistring t_edit" data-name="...">
YetaWF_MultiString.Clear = function($ms)
{
    var name = _YetaWF_MultiString.getName($ms);
    YetaWF_TemplateDropDownList.Clear(_YetaWF_MultiString.getDropDown($ms));
    $("input[type='hidden'][name$='.value']", $ms).val('');
    $("input[name='" + name + "']", $ms).attr('');
    _YetaWF_MultiString.getText($ms).val('');
}

// data is an object with language data for each language
YetaWF_MultiString.ClearData = function (data) {
    var defaultLang = YLocs.YetaWF_ComponentsHTML.Languages[0];
    data = {};
    data[defaultLang] = "";
}

// get the default value from a multistring
YetaWF_MultiString.getDefaultValue = function($ms) {
    var $hidval = _YetaWF_MultiString.getHidden($ms, 0);// default language
    return $hidval.val();
}

// Private
var _YetaWF_MultiString = {};

// get the field name
_YetaWF_MultiString.getName = function($ms) {
    var name = $ms.attr('data-name');
    if (!name) throw "Not a multistring";/*DEBUG*/
    return name;
}
// get the div containing all controls from any tag within the div
_YetaWF_MultiString.getMS = function (obj) {
    var $ms = $(obj).closest('.yt_multistring');
    if ($ms.length != 1) throw "couldn't find parent div";/*DEBUG*/
    return $ms;
}
// get the language specific hidden field
_YetaWF_MultiString.getHidden = function ($ms, sel) {
    var $hidval = $('input[name$=\"[' + sel + '].value\"]', $ms)
    if ($hidval.length != 1) throw "couldn't find input hidden field";/*DEBUG*/
    return $hidval;
}
// get the user visible text box
_YetaWF_MultiString.getText = function ($ms) {
    var $text = $('input.yt_multistring_text', $ms);
    if ($text.length != 1) throw "couldn't find text box";/*DEBUG*/
    return $text;
}
// get the user visible language selection dropdown list
_YetaWF_MultiString.getDropDown = function ($ms) {
    var $dd = $('select', $ms);
    if ($dd.length != 1) throw "couldn't find language dropdown";/*DEBUG*/
    return $dd;
}
// get the index of the selected language
_YetaWF_MultiString.getSelLanguage = function($ms) {
    var $dd = _YetaWF_MultiString.getDropDown($ms);
    return $dd.get(0).selectedIndex;
}

// selection change (put language specific text into text box)
$(document).on("change", '.yt_multistring select', function () {

    var $ms = _YetaWF_MultiString.getMS(this);
    var sel = this.selectedIndex;

    var $hidval = _YetaWF_MultiString.getHidden($ms, sel);
    var $text = _YetaWF_MultiString.getText($ms);
    var newText = $hidval.val();
    if (newText.length == 0 && sel > 0) {
        var $dftlhidval = _YetaWF_MultiString.getHidden($ms, 0);// default language
        var newText = $dftlhidval.val();
        $hidval = _YetaWF_MultiString.getHidden($ms, sel);
        $hidval.val(newText);
    }
    $text.val(newText);
});
// textbox change (save text in language specific hidden fields)
$(document).on("input", '.yt_multistring_text', function () {

    var $ms = _YetaWF_MultiString.getMS(this);
    var sel = _YetaWF_MultiString.getSelLanguage($ms);

    var newText = $(this).val();
    var $hidden = _YetaWF_MultiString.getHidden($ms, sel);
    $hidden.val(newText);

    if (sel == 0)
        YetaWF_TemplateDropDownList.Enable(_YetaWF_MultiString.getDropDown($ms), newText.length > 0);
});
$(document).on("blur", '.yt_multistring_text', function () {

    var $ms = _YetaWF_MultiString.getMS(this);
    var sel = _YetaWF_MultiString.getSelLanguage($ms);

    if (sel == 0) {
        var $hidval = _YetaWF_MultiString.getHidden($ms, 0);
        var text = $hidval.val();
        if (text.length == 0) {
            // the default text was cleared, clear all languages
            var count = YLocs.YetaWF_ComponentsHTML.Languages.length;
            for (var index = 0; index < count ; ++index) {
                var lang = YLocs.YetaWF_ComponentsHTML.Languages[index];
                $hidval = _YetaWF_MultiString.getHidden($ms, index);
                $hidval.val('');
            }
        }
    }
});


