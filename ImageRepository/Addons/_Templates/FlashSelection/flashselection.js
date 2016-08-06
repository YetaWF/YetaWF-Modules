/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ImageRepository#License */

var YetaWF_FlashImageRepository = {};

YetaWF_FlashImageRepository.initSelection = function (divId) {

    var $control = $('#' + divId);
    if ($control.length != 1) throw "div not found";/*DEBUG*/

    var $hidden = $('input[type="hidden"]', $control);
    if ($hidden.length != 1) throw "hidden field not found";/*DEBUG*/

    var $list = $('select[name="List"]', $control);
    if ($list.length != 1) throw "list not found";/*DEBUG*/

    var $buttons = $('.t_haveflash', $buttons);
    if ($buttons.length != 1) throw "button area not found";/*DEBUG*/

    // set the preview flash image
    function setPreview(name) {

        $buttons.toggle(name != null && name.length > 0);

        var $param = $('.t_preview param[name="movie"]', $control);
        var $embed = $('.t_preview embed', $control);

        var $obj = $('.t_preview object', $control);
        if ($obj.length != 1) throw "preview flash object not found";/*DEBUG*/

        // change object data= (if present)
        if ($obj.attr('data') != undefined) {
            var val = $obj.attr('data');
            var currUri = new URI(val);
            currUri.removeSearch("Name");
            currUri.addSearch("Name", name);
            $obj.attr('data', currUri.toString());
        }
        // change param movie (if present)
        if ($param.length > 0) {
            var val = $param.attr('value');
            var currUri = new URI(val);
            currUri.removeSearch("Name");
            currUri.addSearch("Name", name);
            $param.attr('value', currUri.toString());
        }
        // change embed (if present)
        if ($embed.length > 0) {
            var src = $embed.attr('src');
            currUri = new URI(src);
            currUri.removeSearch("Name");
            currUri.addSearch("Name", name);
            $embed.attr('src', currUri.toString());
        }
        var s = $obj[0].outerHTML;
        $obj.replaceWith(s);// replace entire object to make flash recognize the image change
    }
    function clearFileName() {
        $hidden.val('');
        $list.val('');
        setPreview('');
        $('.t_haveflash', $control).toggle(false);
    }
    function successfullUpload(js) {
        $hidden.val(js.filename);
        $list.html(js.list);
        $list.val(js.filename);
        setPreview(js.filename);
        $('.t_haveflash', $control).toggle(js.filename.length > 0);
    }
    // set upload control settings (optional)
    var $upload = $('.yt_fileupload1', $control);
    if ($upload.length == 1) {
        $upload.data({
            getFileName: undefined, // we handle deleting the file
            successfullUpload: successfullUpload,
        });
    }
    // handle the clear button
    $('body').on('click', '#' + divId + ' a[data-name="Clear"]', function (e) {
        e.preventDefault();
        clearFileName();
        return false;
    });
    // handle the remove button
    $('body').on('click', '#' + divId + ' a[data-name="Remove"]', function (e) {
        e.preventDefault();
        var $this = $(this);

        // get url to remove the file
        var href = $this.attr('href');
        var uri = new URI(href);
        uri.removeSearch("Name");
        uri.addSearch("Name", $hidden.val());

        $.ajax({
            url: uri.toString(),
            type: 'post',
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
                var js = JSON.parse(result);
                eval(js.result);
                $list.html(js.list);
                clearFileName();
                return false;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                Y_Alert(YLocs.Forms.AjaxError.format(jqXHR.status, jqXHR.statusText), YLocs.Forms.AjaxErrorTitle);
                return false;
            }
        });
        return false;
    });

    // user changed the selected flash image
    $('body').on('change', '#' + divId + ' select[name="List"]', function () {
        var $this = $(this);
        var name = $this.val();
        setPreview(name);
        $hidden.val(name);
    });
    // show initial selection (if any)
    $('#' + divId + ' select[name="List"]').trigger('change');
};

