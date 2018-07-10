/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

var YetaWF_Url = {};
var _YetaWF_Url = {};

YetaWF_Url.init = function (id) {
    'use strict';
    var $control = $('#' + id);
    if ($control.length != 1) throw "Can't find control";/*DEBUG*/

    var $typeCtl = _YetaWF_Url.getTypeControl($control);
    if ($typeCtl != null) {
        $typeCtl.on('change', function () {
            var $localCtl = _YetaWF_Url.getLocalControl($control);
            var $remoteCtl = _YetaWF_Url.getRemoteControl($control);
            var sel = $typeCtl.val();
            var urlString;
            if (sel == 1)
                urlString = $localCtl.val();
            else
                urlString = $remoteCtl.val();
            YetaWF_Url.Update($control, urlString, false);
            FormsSupport.validateElement($hidden[0]);
        });
    }
    var $localCtl = _YetaWF_Url.getLocalControl($control);
    if ($localCtl != null) {
        $($localCtl).on('change', function () {
            var $hidden = _YetaWF_Url.getHidden($control);
            $hidden.val($localCtl.val());
            _YetaWF_Url.updateLink($control);
            FormsSupport.validateElement($hidden[0]);
            $hidden.trigger('change');// let everyone know value changed
        });
    }
    var $remoteCtl = _YetaWF_Url.getRemoteControl($control);
    if ($remoteCtl != null) {
        $($remoteCtl).on('change', function () {
            var $hidden = _YetaWF_Url.getHidden($control);
            $hidden.val($remoteCtl.val());
            _YetaWF_Url.updateLink($control);
            FormsSupport.validateElement($hidden[0]);
            $hidden.trigger('change');// let everyone know value changed
        });
    }
    // initial selection
    var $hidden = _YetaWF_Url.getHidden($control);
    YetaWF_Url.Update($control, $hidden.val(), true);

    if ($typeCtl) {
        var $localCtl = _YetaWF_Url.getLocalControl($control);
        var $remoteCtl = _YetaWF_Url.getRemoteControl($control);
        if ($localCtl && $remoteCtl)
            YetaWF_TemplateDropDownList.Enable($typeCtl, true);
        else
            YetaWF_TemplateDropDownList.Enable($typeCtl, false);
    }
};

// Enable a url object
// $control refers to the <div class="yt_url t_edit">
YetaWF_Url.Enable = function ($control, enabled) {
    'use strict';
    var $typeCtl = _YetaWF_Url.getTypeControl($control);
    var $localCtl = _YetaWF_Url.getLocalControl($control);
    var $remoteCtl = _YetaWF_Url.getRemoteControl($control);
    if ($localCtl)
        YetaWF_TemplateDropDownList.Enable($localCtl, enabled);
    if ($remoteCtl)
        YetaWF_TemplateText.Enable($remoteCtl, enabled);
    if ($typeCtl) {
        if ($localCtl && $remoteCtl)
            YetaWF_TemplateDropDownList.Enable($typeCtl, enabled);
        else
            YetaWF_TemplateDropDownList.Enable($typeCtl, false);
    }
};
// Load a value into a url object
// $control refers to the <div class="yt_url t_edit">
YetaWF_Url.Update = function ($control, urlString, initial) {
    'use strict';
    var $hidden = _YetaWF_Url.getHidden($control);
    var $typeCtl = _YetaWF_Url.getTypeControl($control);
    var $localCtl = _YetaWF_Url.getLocalControl($control);
    var $remoteCtl = _YetaWF_Url.getRemoteControl($control);
    if ($typeCtl) {
        if ($localCtl && $remoteCtl) {
            var sel = $typeCtl.val();// use selection
            if (urlString != null && (urlString.startsWith('//') || urlString.startsWith('http'))) {
                if ($remoteCtl)
                    sel = 2;
            } else if (initial) {
                YetaWF_TemplateDropDownList.Update($localCtl, urlString);
                var actualSel = $localCtl.val();
                if (urlString != actualSel) {
                    sel = 2; // have to use remote even though it's a local page (but with args)
                } else {
                    sel = 1;
                }
            }
        } else if ($localCtl) {
            sel = 1;
        } else {
            sel = 2;
        }
    } else {
        sel = 2;
    }
    $hidden.val(urlString);
    if ($typeCtl)
        YetaWF_TemplateDropDownList.Update($typeCtl, sel);
    if (sel == 1) {
        if ($localCtl) {
            YetaWF_TemplateDropDownList.Update($localCtl, urlString);
            _YetaWF_Url.getLocalDiv($control).show();
            YetaWF_TemplateDropDownList.initOne($localCtl);// may need to be initialized
        }
        if ($remoteCtl)
            _YetaWF_Url.getRemoteDiv($control).hide();
    } else {
        if ($localCtl)
            _YetaWF_Url.getLocalDiv($control).hide();
        if ($remoteCtl) {
            $remoteCtl.val(urlString);
            _YetaWF_Url.getRemoteDiv($control).show();
            YetaWF_TemplateDropDownList.initOne($remoteCtl);// may need to be initialized
        }
    }
    _YetaWF_Url.updateLink($control);
};
YetaWF_Url.Clear = function ($control) {
    'use strict';
    YetaWF_Url.Update($control, "", true);
};
YetaWF_Url.Retrieve = function ($control) {
    return _YetaWF_Url.getHidden($control).val();
};
YetaWF_Url.RetrieveControl = function ($control) {
    return _YetaWF_Url.getHidden($control);
};
YetaWF_Url.HasChanged = function ($control, data) {
    'use strict';
    var $hidden = _YetaWF_Url.getHidden($control);
    return !StringYCompare(data, $hidden.val());
};


_YetaWF_Url.updateLink = function ($control) {
    'use strict';
    var $hidden = _YetaWF_Url.getHidden($control);
    var $linkCtl = _YetaWF_Url.getLinkCtl($control);
    var urlString = $hidden.val();
    if (urlString == undefined || urlString == "") {
        $linkCtl.hide();
    } else {
        var currUri = YetaWF_Basics.parseUrl(urlString);
        currUri.removeSearch(YConfigs.Basics.Link_NoEditMode);
        currUri.addSearch(YConfigs.Basics.Link_NoEditMode, "y");
        $linkCtl.attr("href", currUri.toUrl());

        $linkCtl.show();
    }
};

// get the hidden field storing the url
_YetaWF_Url.getHidden = function ($control) {
    'use strict';
    var $hidden = $('input[type="hidden"]', $control);
    if ($hidden.length != 1) throw "couldn't find hidden field";/*DEBUG*/
    return $hidden;
};
// get the selection control for the user visible local/remote selector
_YetaWF_Url.getTypeControl = function ($control) {
    'use strict';
    var $sel = $('select.yt_urltype', $control);
    if ($sel.length != 1) return null;
    return $sel;
};
// get the selection from the user visible local/remote selector
_YetaWF_Url.getTypeValue = function ($control) {
    return _YetaWF_Url.getTypeControl($control).val();
};
// get the local page DIV
_YetaWF_Url.getLocalDiv = function ($control) {
    'use strict';
    var $sel = $('.t_local', $control);
    if ($sel.length != 1) throw "couldn't find local div";/*DEBUG*/
    return $sel;
};
// get the local page control
_YetaWF_Url.getLocalControl = function ($control) {
    'use strict';
    var $sel = $('.t_local select', $control);
    if ($sel.length != 1) return null;
    return $sel;
};
// get the remote page control
_YetaWF_Url.getRemoteControl = function ($control) {
    'use strict';
    var $sel = $('.t_remote input', $control);
    if ($sel.length != 1) return null;
    return $sel;
};
// get the remote page DIV
_YetaWF_Url.getRemoteDiv = function ($control) {
    'use strict';
    var $sel = $('.t_remote', $control);
    if ($sel.length != 1) return null;
    return $sel;
};
// get the link to go to when the image is clicked
_YetaWF_Url.getLinkCtl = function ($control) {
    'use strict';
    var $link = $('.t_link a', $control);
    if ($link.length != 1) throw "couldn't find link";/*DEBUG*/
    return $link;
};
