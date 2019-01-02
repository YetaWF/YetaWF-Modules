/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

/* Syntax highlighter autoload */

declare var SyntaxHighlighter: any;

namespace YetaWF_SyntaxHighlighter {

    export interface IPackageLocs {
        msg_expandSource : string;
        msg_help: string;
        msg_alert: string;
        msg_noBrush: string;
        msg_brushNotHtmlScript: string;
        msg_viewSource: string;
        msg_copyToClipboard: string;
        msg_copyToClipboardConfirmation: string;
        msg_print: string;
    }

    class AlexGorbatchevComHighlighterModule {

        static readonly MODULEGUID: string = "7e3c4322-5bdb-44bf-acff-f62d498705ee";

        static on: boolean = true;

        public init(addonUrl: string): void {

            // Autoload doesn't work in ajax, so we have to load all required brushes in page
            // We simply add them to the Addon's filelistJS.txt  TODO: Could add a config page to configure all available brushes
            //_AlexGorbatchevCom_SyntaxHighlighter.url = addon;

            //function path() {
            //    var args = arguments,
            //        result = []
            //    ;

            //    for (var i = 0; i < args.length; i++)
            //        result.push(args[i].replace('@', _AlexGorbatchevCom_SyntaxHighlighter.url + 'scripts/'));

            //    return result
            //};

            //_AlexGorbatchevCom_SyntaxHighlighter.path = path(
            //  'applescript            @shBrushAppleScript.js',
            //  'actionscript3 as3      @shBrushAS3.js',
            //  'bash shell             @shBrushBash.js',
            //  'coldfusion cf          @shBrushColdFusion.js',
            //  'cpp c                  @shBrushCpp.js',
            //  'c# c-sharp csharp      @shBrushCSharp.js',
            //  'css                    @shBrushCss.js',
            //  'delphi pascal          @shBrushDelphi.js',
            //  'diff patch pas         @shBrushDiff.js',
            //  'erl erlang             @shBrushErlang.js',
            //  'groovy                 @shBrushGroovy.js',
            //  'java                   @shBrushJava.js',
            //  'jfx javafx             @shBrushJavaFX.js',
            //  'js jscript javascript  @shBrushJScript.js',
            //  'perl pl                @shBrushPerl.js',
            //  'php                    @shBrushPhp.js',
            //  'text plain             @shBrushPlain.js',
            //  'py python              @shBrushPython.js',
            //  'ruby rails ror rb      @shBrushRuby.js',
            //  'sass scss              @shBrushSass.js',
            //  'scala                  @shBrushScala.js',
            //  'sql                    @shBrushSql.js',
            //  'vb vbnet               @shBrushVb.js',
            //  'xml xhtml xslt html    @shBrushXml.js'
            //);

            //SyntaxHighlighter.autoloader.apply(null, _AlexGorbatchevCom_SyntaxHighlighter.path);

            SyntaxHighlighter.config.strings.expandSource = YLocs.YetaWF_SyntaxHighlighter.msg_expandSource;
            SyntaxHighlighter.config.strings.help = YLocs.YetaWF_SyntaxHighlighter.msg_help;
            SyntaxHighlighter.config.strings.alert = YLocs.YetaWF_SyntaxHighlighter.msg_alert;
            SyntaxHighlighter.config.strings.noBrush = YLocs.YetaWF_SyntaxHighlighter.msg_noBrush;
            SyntaxHighlighter.config.strings.brushNotHtmlScript = YLocs.YetaWF_SyntaxHighlighter.msg_brushNotHtmlScript;
            SyntaxHighlighter.config.strings.viewSource = YLocs.YetaWF_SyntaxHighlighter.msg_viewSource;
            SyntaxHighlighter.config.strings.copyToClipboard = YLocs.YetaWF_SyntaxHighlighter.msg_copyToClipboard;
            SyntaxHighlighter.config.strings.copyToClipboardConfirmation = YLocs.YetaWF_SyntaxHighlighter.msg_copyToClipboardConfirmation;
            SyntaxHighlighter.config.strings.print = YLocs.YetaWF_SyntaxHighlighter.msg_print;

            SyntaxHighlighter.all();

            $YetaWF.registerContentChange((addonGuid: string, on: boolean): void => {
                if (addonGuid === AlexGorbatchevComHighlighterModule.MODULEGUID) {
                    AlexGorbatchevComHighlighterModule.on = on;
                }
            });

            $YetaWF.addWhenReady((tag: HTMLElement) : void => {
                if (AlexGorbatchevComHighlighterModule.on)
                    SyntaxHighlighter.highlight();
            });
        }
    }

    export var AlexGorbatchevCom: AlexGorbatchevComHighlighterModule = new AlexGorbatchevComHighlighterModule();
}

