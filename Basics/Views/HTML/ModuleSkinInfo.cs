/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Basics.Controllers;
using YetaWF.Modules.Basics.Modules;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.Basics.Views {

    public class ModuleSkinInfoView : YetaWFView, IYetaWFView<ModuleSkinInfoModule, ModuleSkinInfoModuleController.DisplayModel> {

        public const string ViewName = "ModuleSkinInfo";

        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        public override string GetViewName() { return ViewName; }

        public async Task<string> RenderViewAsync(ModuleSkinInfoModule module, ModuleSkinInfoModuleController.DisplayModel model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append($@"
{await RenderBeginFormAsync(HtmlAttributes: new { Id = DivId })}
    {await HtmlHelper.ForDisplayContainerAsync(model, "PropertyList")}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType= ButtonTypeEnum.Cancel, Text=this.__ResStr("btnCancel", "Return") },
    })}
{await RenderEndFormAsync()}");

            hb.Append($@"
<div>
    <h1>Testing HTML Element Display</h1>
    <p>This test page is located at http://www.cs.tut.fi/~jkorpela/www/testel.html - credit goes to the author Jukka Korpela.</p>
    <h1>This is a 1st level heading</h1>
    <p>This is a test paragraph.</p>
    <h2>This is a 2nd level heading</h2>
    <p>This is a test paragraph.</p>
    <h3>This is a 3rd level heading</h3>
    <p>This is a test paragraph.</p>
    <h4>This is a 4th level heading</h4>
    <p>This is a test paragraph.</p>
    <h5>This is a 5th level heading</h5>
    <p>This is a test paragraph.</p>
    <h6>This is a 6th level heading</h6>
    <p>This is a test paragraph.</p>

    <h2>Basic block level elements</h2>

    <p>
        This is a normal paragraph (<code>p</code> element).
        To add some length to it, let us mention that this page was
        primarily written for testing the effect of <strong>user style sheets</strong>.
        You can use it for various other purposes as well, like just checking how
        your browser displays various HTML elements by default.
        It can also be useful when testing conversions from HTML
        format to other formats, since some elements can go wrong then.
    </p>
    <p>
        This is another paragraph. I think it needs to be added that
        the set of elements tested is not exhaustive in any sense. I have selected
        those elements for which it can make sense to write user style sheet rules,
        in my opinion.
    </p>
    <div>
        This is a <code>div</code> element. Authors may use such elements instead
        of paragraph markup for various reasons. (End of <code>div</code>.)
    </div>
    <blockquote>
        <p>
            This is a block quotation containing a single
            paragraph. Well, not quite, since this is not <em>really</em>
            quoted text, but I hope you understand the point. After all, this
            page does not use HTML markup very normally anyway.
        </p>
    </blockquote>
    <p>
        The following contains address information about the author, in an <code>address</code>
        element.
    </p>
    <address>
        <a href='http://www.cs.tut.fi/~jkorpela/personal.html' lang='fi' hreflang='en'>Jukka Korpela</a>,
        <a href='mailto:jkorpela@cs.tut.fi'>jkorpela@cs.tut.fi</a><br>
        Päivänsäteenkuja 4 A, Espoo, Finland
    </address>


    <h2>Lists</h2>

    <p>
        This is a paragraph before an <strong>unnumbered</strong> list (<code>ul</code>). Note that
        the spacing between a paragraph and a list before or after that is hard
        to tune in a user style sheet. You can't guess which paragraphs are
        logically related to a list, e.g. as a 'list header'.
    </p>
    <ul>
        <li> One.
        <li> Two.
        <li>
            Three. Well, probably this list item should be longer. Note that
            for short items lists look better if they are compactly presented,
            whereas for long items, it would be better to have more vertical spacing between items.
        <li>
            Four. This is the last item in this list.
            Let us terminate the list now without making any more fuss about it.
    </ul>
    <p>The following is a <code>menu</code> list:</p>
    <menu>
        <li> One.
        <li> Two.
        <li>
            Three. Well, probably this list item should be longer so that it will
            probably wrap to the next line in rendering.
    </menu>
    <p>The following is a <code>dir</code> list:</p>
    <dir>
        <li> One.
        <li> Two.
        <li>
            Three. Well, probably this list item should be longer so that it will
            probably wrap to the next line in rendering.
    </dir>

    <p>
        This is a paragraph before a <strong>numbered</strong> list (<code>ol</code>). Note that
        the spacing between a paragraph and a list before or after that is hard
        to tune in a user style sheet. You can't guess which paragraphs are
        logically related to a list, e.g. as a 'list header'.
    </p>
    <ol>
        <li> One.
        <li> Two.
        <li>
            Three. Well, probably this list item should be longer. Note that if
            items are short, lists look better if they are compactly presented,
            whereas for long items, it would be better to have more vertical spacing between items.
        <li>
            Four. This is the last item in this list.
            Let us terminate the list now without making any more fuss about it.
    </ol>

    <p>
        This is a paragraph before a <strong>definition</strong> list (<code>dl</code>).
        In principle, such a list should consist of <em>terms</em> and associated
        definitions.
        But many authors use <code>dl</code> elements for fancy 'layout' things. Usually the
        effect is not <em>too</em> bad, if you design user style sheet rules for <code>dl</code>
        which are suitable
        for real definition lists.
        <dl>
            <dt> recursion
            <dd> see recursion
            <dt> recursion, indirect
            <dd> see indirect recursion
            <dt> indirect recursion
            <dd> see recursion, indirect
            <dt> term
            <dd>
                a word or other expression taken into specific use in
                a well-defined meaning, which is often defined rather rigorously, even
                formally, and may differ quite a lot from an everyday meaning
        </dl>

        <h2>Text-level markup</h2>

        <ul>
            <li>
                <abbr title='Cascading Style Sheets'>CSS</abbr> (an abbreviation;
                <code>abbr</code> markup used)
            <li> <acronym title='radio detecting and ranging'>radar</acronym> (an acronym; <code>acronym</code> markup used)
            <li>
                <b>bolded</b> (<code>b</code> markup used - just bolding with unspecified
                semantics)
            <li> <big>big thing</big> (<code>big</code> markup used)
            <li> <font size=6>large size</font> (<code>font size=6</code> markup used)
            <li> <font face=Courier>Courier font</font> (<code>font face=Courier</code> markup used)
            <li> <font color=red>red text</font> (<code>font color=red</code> markup used)
            <li>
                <cite>Origin of Species</cite> (a book title;
                <code>cite</code> markup used)
            <li> <code>a[i] = b[i] + c[i);</code> (computer code; <code>code</code> markup used)
            <li> here we have some <del>deleted</del> text (<code>del</code> markup used)
            <li>
                an <dfn>octet</dfn> is an entity consisting of eight bits
                (<code>dfn</code> markup used for the term being defined)
            <li>
                this is <em>very</em> simple (<code>em</code> markup used for emphasizing
                a word)
            <li> <i lang='la'>Homo sapiens</i> (should appear in italics;  <code>i</code> markup used)
            <li> here we have some <ins>inserted</ins> text (<code>ins</code> markup used)
            <li>
                type <kbd>yes</kbd> when prompted for an answer (<code>kbd</code> markup
                used for text indicating keyboard input)
            <li> <q>Hello!</q> (<code>q</code> markup used for quotation)
            <li> He said: <q>She said <q>Hello!</q></q> (a quotation inside a quotation)
            <li>
                you may get the message <samp>Core dumped</samp> at times
                (<code>samp</code> markup used for sample output)
            <li> <small>this is not that important</small> (<code>small</code> markup used)
            <li>
                <strike>overstruck</strike> (<code>strike</code> markup used; note:
                <code>s</code> is a nonstandard synonym for <code>strike</code>)
            <li>
                <strong>this is highlighted text</strong> (<code>strong</code>
                markup used)
            <li>
                In order to test how subscripts and superscripts (<code>sub</code> and
                <code>sup</code> markup) work inside running text, we need some
                dummy text around constructs like x<sub>1</sub> and H<sub>2</sub>O
                (where subscripts occur). So here is some fill so that
                you will (hopefully) see whether and how badly the subscripts
                and superscripts mess up vertical spacing between lines.
                Now superscripts: M<sup>lle</sup>, 1<sup>st</sup>, and then some
                mathematical notations: e<sup>x</sup>, sin<sup>2</sup> <i>x</i>,
                and some nested superscripts (exponents) too:
                e<sup>x<sup>2</sup></sup> and f(x)<sup>g(x)<sup>a+b+c</sup></sup>
                (where 2 and a+b+c should appear as exponents of exponents).
            <li> <tt>text in monospace font</tt> (<code>tt</code> markup used)
            <li> <u>underlined</u> text (<code>u</code> markup used)
            <li>
                the command <code>cat</code> <var>filename</var> displays the
                file specified by the <var>filename</var> (<code>var</code> markup
                used to indicate a word as a variable).
        </ul>

    <p>
        Some of the elements tested above are typically displayed in a monospace
        font, often using the <em>same</em> presentation for all of them. This
        tests whether that is the case on your browser:
    </p>

    <ul>
        <li> <code>This is sample text inside code markup</code>
        <li> <kbd>This is sample text inside kbd markup</kbd>
        <li> <samp>This is sample text inside samp markup</samp>
        <li> <tt>This is sample text inside tt markup</tt>
    </ul>
    <h2>Links</h2>
    <ul>
        <li> <a href='http://www.cs.tut.fi/~jkorpela/index.html'>main page</a>
        <li>
            <a href='http://www.unicode.org/versions/Unicode4.0.0/ch06.pdf'
                title='Writing Systems and Punctuation'
                type='application/pdf'>Unicode Standard, chapter&nbsp;6</a>
    </ul>

    <p>
        This is a text paragraph that contains some
        inline links. Generally, inline links (as opposite to e.g. links
        lists) are problematic
        from the
        <a href='http://www.useit.com'>usability</a> perspective,
        but they may have use as
        &#8220;incidental&#8221;, less relevant links. See the document
        <cite><a href='http://www.cs.tut.fi/~jkorpela/www/links.html'>Links Want To Be Links</a></cite>.
    </p>

    <h2>Forms</h2>

    <form action='http://www.cs.tut.fi/cgi-bin/run/~jkorpela/echo.cgi'>
        <div>
            <input type='hidden' name='hidden field' value='42'>
            This is a form containing various fields (with some initial
            values (defaults) set, so that you can see how input text looks
            like without actually typing it):
        </div>
        <div>
            <label for='but'>
                Button:
                <button id='but' type='submit' name='foo' value='bar'>A cool<br>button</button>
            </label>
        </div>
        <div>
            <label for='f0'>
                Reset button:
                <input id='f0' type='reset' name='reset' value='Reset'>
            </label>
        </div>
        <div><label for='f1'>Single-line text input field: <input id='f1' name='text' size='20' value='Default text.'></label></div>
        <div>
            <label for='f2'>Multi-line text input field (textarea):</label><br>
            <textarea id='f2' name='textarea' rows='2' cols='20'>
                Default text.
            </textarea>
        </div>
        <div>
            The following two radio buttons are inside
            a <code>fieldset</code> element with a <code>legend</code>:
        </div>
        <fieldset>
            <legend>Legend</legend>
            <div><label for='f3'><input id='f3' type='radio' name='radio' value='1'> Radio button 1</label></div>
            <div><label for='f4'><input id='f4' type='radio' name='radio' value='2' checked> Radio button 2 (initially checked)</label></div>
        </fieldset>
        <fieldset>
            <legend>Check those that apply</legend>
            <div><label for='f5'><input id='f5' type='checkbox' name='checkbox'> Checkbox 1</label></div>
            <div><label for='f6'><input id='f6' type='checkbox' name='checkbox2' checked> Checkbox 2 (initially checked)</label></div>
        </fieldset>
        <div>
            <label for='f10'>
                A <code>select</code> element with <code>size='1'</code>
                (dropdown box):
                <select class='t_native' id='f10' name='select1' size='1'>
                    <option>one</option>
                    <option selected>two (default)</option>
                    <option>three</option>
                </select>
            </label>
        </div>
        <div>
            <label for='f11'>
                A <code>select</code> element with <code>size='3'</code>
                (listbox):
            </label><br>
            <select class='t_native' id='f11' name='select2' size='3'>
                <option>one</option>
                <option selected>two (default)</option>
                <option>three</option>
            </select>
        </div>
        <div>
            <label for='f99'>
                Submit button:
                <input id='f99' type='submit' name='submit' value='Just a test'>
            </label>
        </div>
    </form>

    <h2>Tables</h2>

    <p>
        The following table has a caption. The first row and the first column
        contain table header cells (<code>th</code> elements) only; other cells
        are data cells (<code>td</code> elements), with <code>align='right'</code>
        attributes:
    </p>

    <table summary='Each row names a Nordic country and specifies its total area and land area, in square kilometers'>
        <caption>Sample table: Areas of the Nordic countries, in sq km</caption>
        <tr><th scope='col'>Country</th> <th scope='col'>Total area</th> <th scope='col'>Land area</th>
        <tr><th scope='row'>Denmark</th> <td align=RIGHT> 43,070 </td><td align=RIGHT> 42,370</tr>
        <tr><th scope='row'>Finland</th> <td align=RIGHT>337,030 </td><td align=RIGHT>305,470</tr>
        <tr><th scope='row'>Iceland</th> <td align=RIGHT>103,000 </td><td align=RIGHT>100,250</tr>
        <tr><th scope='row'>Norway</th>  <td align=RIGHT>324,220 </td><td align=RIGHT>307,860</tr>
        <tr><th scope='row'>Sweden</th>  <td align=RIGHT>449,964 </td><td align=RIGHT>410,928</tr>
    </table>

    <h2>Character test</h2>
    <p>
        The following table has some sample characters with
        annotations. If the browser&#8217;s default font does not
        contain all of them, they may get displayed using backup fonts.
        This may cause stylistic differences, but it should not
        prevent the characters from being displayed at all.
    </p>

    <table>
        <tr><th>Char. <th>Explanation <th>Notes
        <tr><td>ê <td>e with circumflex <td>Latin 1 character, should be ok
        <tr><td>&#8212; <td>em dash <td>Windows Latin 1 character, should be ok, too
        <tr><td>&#x100; <td>A with macron (line above) <td>Latin Extended-A character, not present in all fonts
        <tr><td>&Omega; <td>capital omega <td>A Greek letter
        <tr><td>&#x2212; <td>minus sign <td>Unicode minus
        <tr><td>&#x2300; <td>diameter sign <td>relatively rare in fonts
    </table>

</div>");

            return hb.ToString();
        }
    }
}
