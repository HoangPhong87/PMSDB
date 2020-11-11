/*
	99Lime.com HTML KickStart by Joshua Gatcke
	kickstart.js
*/

jQuery(document).ready(function ($) {
    /*---------------------------------
        Table Sort
    -----------------------------------*/
    // init
    var aAsc = [];
    $('table.sortable').each(function () {
        $(this).find('thead th').each(function (index) { $(this).attr('rel', index); });
        $(this).find('th,td').each(function () { $(this).attr('value', $(this).text()); });
    });

    // table click
    $(document).on('click', 'table.sortable thead th', function (e) {
        // update arrow icon
        $(this).parents('table.sortable').find('span.arrow').remove();
        $(this).append('<span class="arrow"></span>');

        // sort direction
        var nr = $(this).attr('rel');
        aAsc[nr] = aAsc[nr] == 'asc' ? 'desc' : 'asc';
        if (aAsc[nr] == 'desc') { $(this).find('span.arrow').addClass('up'); }

        // sort rows
        var rows = $(this).parents('table.sortable').find('tbody tr');
        rows.tsort('td:eq(' + nr + ')', { order: aAsc[nr], attr: 'value' });

        // fix row classes
        rows.removeClass('alt first last');
        //var table = $(this).parents('table.sortable');
        //table.find('tr:even').addClass('alt');
        //table.find('tr:first').addClass('first');
        //table.find('tr:last').addClass('last');
    });

    ///*---------------------------------
    //    CSS Helpers
    //-----------------------------------*/
    //$('input[type=checkbox]').addClass('checkbox');
    //$('input[type=radio]').addClass('radio');
    //$('input[type=file]').addClass('file');
    //$('[disabled=disabled]').addClass('disabled');
    //$('table').find('tr:even').addClass('alt');
    //$('table').find('tr:first-child').addClass('first');
    //$('table').find('tr:last-child').addClass('last');
    //$('ul').find('li:first-child').addClass('first');
    //$('ul').find('li:last-child').addClass('last');
    //$('hr').before('<div class="clear">&nbsp;</div>');
    //$('[class*="col_"]').addClass('column');
    //$('pre').addClass('prettyprint');prettyPrint();

    $(document).on('click', '.show-advance-search span', function (e) {
        $('.advance-search').stop(true).slideToggle('fast');

        if ($(this).hasClass('action-show-adv-search')) {
            $(this).hide();
            $('.action-hide-adv-search').show();
        } else {
            $(this).hide();
            $('.action-show-adv-search').show();
        }
    });
});

/*
 * FancyBox - jQuery Plugin
 * Simple and fancy lightbox alternative
 *
 * Examples and documentation at: http://fancybox.net
 *
 * Copyright (c) 2008 - 2010 Janis Skarnelis
 * That said, it is hardly a one-person project. Many people have submitted bugs, code, and offered their advice freely. Their support is greatly appreciated.
 *
 * Version: 1.3.4 (11/11/2010)
 * Requires: jQuery v1.3+
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 */
(function (a) {
    var p, u, v, e, B, m, C, j, y, z, s = 0, d = {}, q = [], r = 0, c = {}, k = [], E = null, n = new Image, H = /\.(jpg|gif|png|bmp|jpeg)(.*)?$/i, S = /[^\.]\.(swf)\s*$/i, I, J = 1, x = 0, w = "", t, g, l = !1, A = a.extend(a("<div/>")[0], { prop: 0 }), K = navigator.userAgent.match(/msie [6]/i) && !window.XMLHttpRequest, L = function () { u.hide(); n.onerror = n.onload = null; E && E.abort(); p.empty() }, M = function () {
        !1 === d.onError(q, s, d) ? (u.hide(), l = !1) : (d.titleShow = !1, d.width = "auto", d.height = "auto", p.html('<p id="fancybox-error">The requested content cannot be loaded.<br />Please try again later.</p>'),
        D())
    }, G = function () {
        var b = q[s], c, f, e, g, k, j; L(); d = a.extend({}, a.fn.fancybox.defaults, "undefined" == typeof a(b).data("fancybox") ? d : a(b).data("fancybox")); j = d.onStart(q, s, d); if (!1 === j) l = !1; else {
            "object" == typeof j && (d = a.extend(d, j)); e = d.title || (b.nodeName ? a(b).attr("title") : b.title) || ""; b.nodeName && !d.orig && (d.orig = a(b).children("img:first").length ? a(b).children("img:first") : a(b)); "" === e && (d.orig && d.titleFromAlt) && (e = d.orig.attr("alt")); c = d.href || (b.nodeName ? a(b).attr("href") : b.href) || null; if (/^(?:javascript)/i.test(c) ||
            "#" == c) c = null; d.type ? (f = d.type, c || (c = d.content)) : d.content ? f = "html" : c && (f = c.match(H) ? "image" : c.match(S) ? "swf" : a(b).hasClass("iframe") ? "iframe" : 0 === c.indexOf("#") ? "inline" : "ajax"); if (f) switch ("inline" == f && (b = c.substr(c.indexOf("#")), f = 0 < a(b).length ? "inline" : "ajax"), d.type = f, d.href = c, d.title = e, d.autoDimensions && ("html" == d.type || "inline" == d.type || "ajax" == d.type ? (d.width = "auto", d.height = "auto") : d.autoDimensions = !1), d.modal && (d.overlayShow = !0, d.hideOnOverlayClick = !1, d.hideOnContentClick = !1, d.enableEscapeButton =
            !1, d.showCloseButton = !1), d.padding = parseInt(d.padding, 10), d.margin = parseInt(d.margin, 10), p.css("padding", d.padding + d.margin), a(".fancybox-inline-tmp").unbind("fancybox-cancel").bind("fancybox-change", function () { a(this).replaceWith(m.children()) }), f) {
                case "html": p.html(d.content); D(); break; case "inline": if (!0 === a(b).parent().is("#fancybox-content")) { l = !1; break } a('<div class="fancybox-inline-tmp" />').hide().insertBefore(a(b)).bind("fancybox-cleanup", function () { a(this).replaceWith(m.children()) }).bind("fancybox-cancel",
                function () { a(this).replaceWith(p.children()) }); a(b).appendTo(p); D(); break; case "image": l = !1; a.fancybox.showActivity(); n = new Image; n.onerror = function () { M() }; n.onload = function () { l = !0; n.onerror = n.onload = null; d.width = n.width; d.height = n.height; a("<img />").attr({ id: "fancybox-img", src: n.src, alt: d.title }).appendTo(p); N() }; n.src = c; break; case "swf": d.scrolling = "no"; g = '<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" width="' + d.width + '" height="' + d.height + '"><param name="movie" value="' + c + '"></param>';
                    k = ""; a.each(d.swf, function (a, b) { g += '<param name="' + a + '" value="' + b + '"></param>'; k += " " + a + '="' + b + '"' }); g += '<embed src="' + c + '" type="application/x-shockwave-flash" width="' + d.width + '" height="' + d.height + '"' + k + "></embed></object>"; p.html(g); D(); break; case "ajax": l = !1; a.fancybox.showActivity(); d.ajax.win = d.ajax.success; E = a.ajax(a.extend({}, d.ajax, {
                        url: c, data: d.ajax.data || {}, error: function (a) { 0 < a.status && M() }, success: function (a, b, f) {
                            if (200 == ("object" == typeof f ? f : E).status) {
                                if ("function" == typeof d.ajax.win) {
                                    j =
                                    d.ajax.win(c, a, b, f); if (!1 === j) { u.hide(); return } if ("string" == typeof j || "object" == typeof j) a = j
                                } p.html(a); D()
                            }
                        }
                    })); break; case "iframe": N()
            } else M()
        }
    }, D = function () {
        var b = d.width, c = d.height, b = -1 < b.toString().indexOf("%") ? parseInt((a(window).width() - 2 * d.margin) * parseFloat(b) / 100, 10) + "px" : "auto" == b ? "auto" : b + "px", c = -1 < c.toString().indexOf("%") ? parseInt((a(window).height() - 2 * d.margin) * parseFloat(c) / 100, 10) + "px" : "auto" == c ? "auto" : c + "px"; p.wrapInner('<div style="width:' + b + ";height:" + c + ";overflow: " + ("auto" ==
        d.scrolling ? "auto" : "yes" == d.scrolling ? "scroll" : "hidden") + ';position:relative;"></div>'); d.width = p.width(); d.height = p.height(); N()
    }, N = function () {
        var b, h; u.hide(); if (e.is(":visible") && !1 === c.onCleanup(k, r, c)) a.event.trigger("fancybox-cancel"), l = !1; else {
            l = !0; a(m.add(v)).unbind(); a(window).unbind("resize.fb scroll.fb"); a(document).unbind("keydown.fb"); e.is(":visible") && "outside" !== c.titlePosition && e.css("height", e.height()); k = q; r = s; c = d; if (c.overlayShow) {
                if (v.css({
                "background-color": c.overlayColor, opacity: c.overlayOpacity,
                    cursor: c.hideOnOverlayClick ? "pointer" : "auto", height: a(document).height()
                }), !v.is(":visible")) { if (K) a("select:not(#fancybox-tmp select)").filter(function () { return "hidden" !== this.style.visibility }).css({ visibility: "hidden" }).one("fancybox-cleanup", function () { this.style.visibility = "inherit" }); v.show() }
            } else v.hide(); b = O(); var f = {}, F = c.autoScale, n = 2 * c.padding; f.width = -1 < c.width.toString().indexOf("%") ? parseInt(b[0] * parseFloat(c.width) / 100, 10) : c.width + n; f.height = -1 < c.height.toString().indexOf("%") ? parseInt(b[1] *
            parseFloat(c.height) / 100, 10) : c.height + n; if (F && (f.width > b[0] || f.height > b[1])) "image" == d.type || "swf" == d.type ? (F = c.width / c.height, f.width > b[0] && (f.width = b[0], f.height = parseInt((f.width - n) / F + n, 10)), f.height > b[1] && (f.height = b[1], f.width = parseInt((f.height - n) * F + n, 10))) : (f.width = Math.min(f.width, b[0]), f.height = Math.min(f.height, b[1])); f.top = parseInt(Math.max(b[3] - 20, b[3] + 0.5 * (b[1] - f.height - 40)), 10); f.left = parseInt(Math.max(b[2] - 20, b[2] + 0.5 * (b[0] - f.width - 40)), 10); g = f; w = c.title || ""; x = 0; j.empty().removeAttr("style").removeClass();
            if (!1 !== c.titleShow && (w = a.isFunction(c.titleFormat) ? c.titleFormat(w, k, r, c) : w && w.length ? "float" == c.titlePosition ? '<table id="fancybox-title-float-wrap" cellpadding="0" cellspacing="0"><tr><td id="fancybox-title-float-left"></td><td id="fancybox-title-float-main">' + w + '</td><td id="fancybox-title-float-right"></td></tr></table>' : '<div id="fancybox-title-' + c.titlePosition + '">' + w + "</div>" : !1) && "" !== w) switch (j.addClass("fancybox-title-" + c.titlePosition).html(w).appendTo("body").show(), c.titlePosition) {
                case "inside": j.css({
                    width: g.width -
                    2 * c.padding, marginLeft: c.padding, marginRight: c.padding
                }); x = j.outerHeight(!0); j.appendTo(B); g.height += x; break; case "over": j.css({ marginLeft: c.padding, width: g.width - 2 * c.padding, bottom: c.padding }).appendTo(B); break; case "float": j.css("left", -1 * parseInt((j.width() - g.width - 40) / 2, 10)).appendTo(e); break; default: j.css({ width: g.width - 2 * c.padding, paddingLeft: c.padding, paddingRight: c.padding }).appendTo(e)
            } j.hide(); e.is(":visible") ? (a(C.add(y).add(z)).hide(), b = e.position(), t = {
                top: b.top, left: b.left, width: e.width(),
                height: e.height()
            }, h = t.width == g.width && t.height == g.height, m.fadeTo(c.changeFade, 0.3, function () { var b = function () { m.html(p.contents()).fadeTo(c.changeFade, 1, P) }; a.event.trigger("fancybox-change"); m.empty().removeAttr("filter").css({ "border-width": c.padding, width: g.width - 2 * c.padding, height: d.autoDimensions ? "auto" : g.height - x - 2 * c.padding }); h ? b() : (A.prop = 0, a(A).animate({ prop: 1 }, { duration: c.changeSpeed, easing: c.easingChange, step: Q, complete: b })) })) : (e.removeAttr("style"), m.css("border-width", c.padding), "elastic" ==
            c.transitionIn ? (t = R(), m.html(p.contents()), e.show(), c.opacity && (g.opacity = 0), A.prop = 0, a(A).animate({ prop: 1 }, { duration: c.speedIn, easing: c.easingIn, step: Q, complete: P })) : ("inside" == c.titlePosition && 0 < x && j.show(), m.css({ width: g.width - 2 * c.padding, height: d.autoDimensions ? "auto" : g.height - x - 2 * c.padding }).html(p.contents()), e.css(g).fadeIn("none" == c.transitionIn ? 0 : c.speedIn, P)))
        }
    }, P = function () {
        a.support.opacity || (m.get(0).style.removeAttribute("filter"), e.get(0).style.removeAttribute("filter")); d.autoDimensions &&
        m.css("height", "auto"); e.css("height", "auto"); w && w.length && j.show(); c.showCloseButton && C.show(); (c.enableEscapeButton || c.enableKeyboardNav) && a(document).bind("keydown.fb", function (b) { if (27 == b.keyCode && c.enableEscapeButton) b.preventDefault(), a.fancybox.close(); else if ((37 == b.keyCode || 39 == b.keyCode) && c.enableKeyboardNav && "INPUT" !== b.target.tagName && "TEXTAREA" !== b.target.tagName && "SELECT" !== b.target.tagName) b.preventDefault(), a.fancybox[37 == b.keyCode ? "prev" : "next"]() }); c.showNavArrows ? ((c.cyclic && 1 <
        k.length || 0 !== r) && y.show(), (c.cyclic && 1 < k.length || r != k.length - 1) && z.show()) : (y.hide(), z.hide()); c.hideOnContentClick && m.bind("click", a.fancybox.close); c.hideOnOverlayClick && v.bind("click", a.fancybox.close); a(window).bind("resize.fb", a.fancybox.resize); c.centerOnScroll && a(window).bind("scroll.fb", a.fancybox.center); "iframe" == c.type && a('<iframe id="fancybox-frame" name="fancybox-frame' + (new Date).getTime() + '" frameborder="0" hspace="0" ' + (navigator.userAgent.match(/msie [6]/i) ? 'allowtransparency="true""' :
        "") + ' scrolling="' + d.scrolling + '" src="' + c.href + '"></iframe>').appendTo(m); e.show(); l = !1; a.fancybox.center(); c.onComplete(k, r, c); var b, h; k.length - 1 > r && (b = k[r + 1].href, "undefined" !== typeof b && b.match(H) && (h = new Image, h.src = b)); 0 < r && (b = k[r - 1].href, "undefined" !== typeof b && b.match(H) && (h = new Image, h.src = b))
    }, Q = function (b) {
        var a = {
            width: parseInt(t.width + (g.width - t.width) * b, 10), height: parseInt(t.height + (g.height - t.height) * b, 10), top: parseInt(t.top + (g.top - t.top) * b, 10), left: parseInt(t.left + (g.left - t.left) * b,
            10)
        }; "undefined" !== typeof g.opacity && (a.opacity = 0.5 > b ? 0.5 : b); e.css(a); m.css({ width: a.width - 2 * c.padding, height: a.height - x * b - 2 * c.padding })
    }, O = function () { return [a(window).width() - 2 * c.margin, a(window).height() - 2 * c.margin, a(document).scrollLeft() + c.margin, a(document).scrollTop() + c.margin] }, R = function () {
        var b = d.orig ? a(d.orig) : !1, h = {}; b && b.length ? (h = b.offset(), h.top += parseInt(b.css("paddingTop"), 10) || 0, h.left += parseInt(b.css("paddingLeft"), 10) || 0, h.top += parseInt(b.css("border-top-width"), 10) || 0, h.left +=
        parseInt(b.css("border-left-width"), 10) || 0, h.width = b.width(), h.height = b.height(), h = { width: h.width + 2 * c.padding, height: h.height + 2 * c.padding, top: h.top - c.padding - 20, left: h.left - c.padding - 20 }) : (b = O(), h = { width: 2 * c.padding, height: 2 * c.padding, top: parseInt(b[3] + 0.5 * b[1], 10), left: parseInt(b[2] + 0.5 * b[0], 10) }); return h
    }, T = function () { u.is(":visible") ? (a("div", u).css("top", -40 * J + "px"), J = (J + 1) % 12) : clearInterval(I) }; a.fn.fancybox = function (b) {
        if (!a(this).length) return this; a(this).data("fancybox", a.extend({}, b, a.metadata ?
        a(this).metadata() : {})).unbind("click.fb").bind("click.fb", function (b) { b.preventDefault(); l || (l = !0, a(this).blur(), q = [], s = 0, b = a(this).attr("rel") || "", !b || "" == b || "nofollow" === b ? q.push(this) : (q = a("a[rel=" + b + "], area[rel=" + b + "], img[rel=" + b + "]"), s = q.index(this)), G()) }); return this
    }; a.fancybox = function (b, c) {
        var d; if (!l) {
            l = !0; d = "undefined" !== typeof c ? c : {}; q = []; s = parseInt(d.index, 10) || 0; if (a.isArray(b)) {
                for (var e = 0, g = b.length; e < g; e++) "object" == typeof b[e] ? a(b[e]).data("fancybox", a.extend({}, d, b[e])) : b[e] =
                a({}).data("fancybox", a.extend({ content: b[e] }, d)); q = jQuery.merge(q, b)
            } else "object" == typeof b ? a(b).data("fancybox", a.extend({}, d, b)) : b = a({}).data("fancybox", a.extend({ content: b }, d)), q.push(b); if (s > q.length || 0 > s) s = 0; G()
        }
    }; a.fancybox.showActivity = function () { clearInterval(I); u.show(); I = setInterval(T, 66) }; a.fancybox.hideActivity = function () { u.hide() }; a.fancybox.next = function () { return a.fancybox.pos(r + 1) }; a.fancybox.prev = function () { return a.fancybox.pos(r - 1) }; a.fancybox.pos = function (b) {
        l || (b = parseInt(b),
        q = k, -1 < b && b < k.length ? (s = b, G()) : c.cyclic && 1 < k.length && (s = b >= k.length ? 0 : k.length - 1, G()))
    }; a.fancybox.cancel = function () { l || (l = !0, a.event.trigger("fancybox-cancel"), L(), d.onCancel(q, s, d), l = !1) }; a.fancybox.close = function () {
        function b() { v.fadeOut("fast"); j.empty().hide(); e.hide(); a.event.trigger("fancybox-cleanup"); m.empty(); c.onClosed(k, r, c); k = d = []; r = s = 0; c = d = {}; l = !1 } if (!l && !e.is(":hidden")) if (l = !0, c && !1 === c.onCleanup(k, r, c)) l = !1; else if (L(), a(C.add(y).add(z)).hide(), a(m.add(v)).unbind(), a(window).unbind("resize.fb scroll.fb"),
        a(document).unbind("keydown.fb"), m.find("iframe").attr("src", K && /^https/i.test(window.location.href || "") ? "javascript:void(false)" : "about:blank"), "inside" !== c.titlePosition && j.empty(), e.stop(), "elastic" == c.transitionOut) { t = R(); var h = e.position(); g = { top: h.top, left: h.left, width: e.width(), height: e.height() }; c.opacity && (g.opacity = 1); j.empty().hide(); A.prop = 1; a(A).animate({ prop: 0 }, { duration: c.speedOut, easing: c.easingOut, step: Q, complete: b }) } else e.fadeOut("none" == c.transitionOut ? 0 : c.speedOut, b)
    }; a.fancybox.resize =
    function () { v.is(":visible") && v.css("height", a(document).height()); a.fancybox.center(!0) }; a.fancybox.center = function (b) { var a, d; if (!l && (d = !0 === b ? 1 : 0, a = O(), d || !(e.width() > a[0] || e.height() > a[1]))) e.stop().animate({ top: parseInt(Math.max(a[3] - 20, a[3] + 0.5 * (a[1] - m.height() - 40) - c.padding)), left: parseInt(Math.max(a[2] - 20, a[2] + 0.5 * (a[0] - m.width() - 40) - c.padding)) }, "number" == typeof b ? b : 200) }; a.fancybox.init = function () {
        a("#fancybox-wrap").length || (a("body").append(p = a('<div id="fancybox-tmp"></div>'), u = a('<div id="fancybox-loading"><div></div></div>'),
        v = a('<div id="fancybox-overlay"></div>'), e = a('<div id="fancybox-wrap"></div>')), B = a('<div id="fancybox-outer"></div>').append('<div class="fancybox-bg" id="fancybox-bg-n"></div><div class="fancybox-bg" id="fancybox-bg-ne"></div><div class="fancybox-bg" id="fancybox-bg-e"></div><div class="fancybox-bg" id="fancybox-bg-se"></div><div class="fancybox-bg" id="fancybox-bg-s"></div><div class="fancybox-bg" id="fancybox-bg-sw"></div><div class="fancybox-bg" id="fancybox-bg-w"></div><div class="fancybox-bg" id="fancybox-bg-nw"></div>').appendTo(e),
        B.append(m = a('<div id="fancybox-content"></div>'), C = a('<a id="fancybox-close"></a>'), j = a('<div id="fancybox-title"></div>'), y = a('<a href="javascript:;" id="fancybox-left"><span class="fancy-ico" id="fancybox-left-ico"></span></a>'), z = a('<a href="javascript:;" id="fancybox-right"><span class="fancy-ico" id="fancybox-right-ico"></span></a>')), C.click(a.fancybox.close), u.click(a.fancybox.cancel), y.click(function (b) { b.preventDefault(); a.fancybox.prev() }), z.click(function (b) { b.preventDefault(); a.fancybox.next() }),
        a.fn.mousewheel && e.bind("mousewheel.fb", function (b, c) { if (l) b.preventDefault(); else if (0 == a(b.target).get(0).clientHeight || a(b.target).get(0).scrollHeight === a(b.target).get(0).clientHeight) b.preventDefault(), a.fancybox[0 < c ? "prev" : "next"]() }), a.support.opacity || e.addClass("fancybox-ie"), K && (u.addClass("fancybox-ie6"), e.addClass("fancybox-ie6"), a('<iframe id="fancybox-hide-sel-frame" src="' + (/^https/i.test(window.location.href || "") ? "javascript:void(false)" : "about:blank") + '" scrolling="no" border="0" frameborder="0" tabindex="-1"></iframe>').prependTo(B)))
    };
    a.fn.fancybox.defaults = {
        padding: 10, margin: 40, opacity: !1, modal: !1, cyclic: !1, scrolling: "auto", width: 560, height: 340, autoScale: !0, autoDimensions: !0, centerOnScroll: !1, ajax: {}, swf: { wmode: "transparent" }, hideOnOverlayClick: !0, hideOnContentClick: !1, overlayShow: !0, overlayOpacity: 0.7, overlayColor: "#777", titleShow: !0, titlePosition: "float", titleFormat: null, titleFromAlt: !1, transitionIn: "fade", transitionOut: "fade", speedIn: 300, speedOut: 300, changeSpeed: 300, changeFade: "fast", easingIn: "swing", easingOut: "swing", showCloseButton: !0,
        showNavArrows: !0, enableEscapeButton: !0, enableKeyboardNav: !0, onStart: function () { }, onCancel: function () { }, onComplete: function () { }, onCleanup: function () { }, onClosed: function () { }, onError: function () { }
    }; a(document).ready(function () { a.fancybox.init() })
})(jQuery);

/*
	Prettify JS
*/
var q = null; window.PR_SHOULD_USE_CONTINUATION = !0;
(function () {
    function L(a) {
        function m(a) { var f = a.charCodeAt(0); if (f !== 92) return f; var b = a.charAt(1); return (f = r[b]) ? f : "0" <= b && b <= "7" ? parseInt(a.substring(1), 8) : b === "u" || b === "x" ? parseInt(a.substring(2), 16) : a.charCodeAt(1) } function e(a) { if (a < 32) return (a < 16 ? "\\x0" : "\\x") + a.toString(16); a = String.fromCharCode(a); if (a === "\\" || a === "-" || a === "[" || a === "]") a = "\\" + a; return a } function h(a) {
            for (var f = a.substring(1, a.length - 1).match(/\\u[\dA-Fa-f]{4}|\\x[\dA-Fa-f]{2}|\\[0-3][0-7]{0,2}|\\[0-7]{1,2}|\\[\S\s]|[^\\]/g), a =
            [], b = [], o = f[0] === "^", c = o ? 1 : 0, i = f.length; c < i; ++c) { var j = f[c]; if (/\\[bdsw]/i.test(j)) a.push(j); else { var j = m(j), d; c + 2 < i && "-" === f[c + 1] ? (d = m(f[c + 2]), c += 2) : d = j; b.push([j, d]); d < 65 || j > 122 || (d < 65 || j > 90 || b.push([Math.max(65, j) | 32, Math.min(d, 90) | 32]), d < 97 || j > 122 || b.push([Math.max(97, j) & -33, Math.min(d, 122) & -33])) } } b.sort(function (a, f) { return a[0] - f[0] || f[1] - a[1] }); f = []; j = [NaN, NaN]; for (c = 0; c < b.length; ++c) i = b[c], i[0] <= j[1] + 1 ? j[1] = Math.max(j[1], i[1]) : f.push(j = i); b = ["["]; o && b.push("^"); b.push.apply(b, a); for (c = 0; c <
            f.length; ++c) i = f[c], b.push(e(i[0])), i[1] > i[0] && (i[1] + 1 > i[0] && b.push("-"), b.push(e(i[1]))); b.push("]"); return b.join("")
        } function y(a) {
            for (var f = a.source.match(/\[(?:[^\\\]]|\\[\S\s])*]|\\u[\dA-Fa-f]{4}|\\x[\dA-Fa-f]{2}|\\\d+|\\[^\dux]|\(\?[!:=]|[()^]|[^()[\\^]+/g), b = f.length, d = [], c = 0, i = 0; c < b; ++c) { var j = f[c]; j === "(" ? ++i : "\\" === j.charAt(0) && (j = +j.substring(1)) && j <= i && (d[j] = -1) } for (c = 1; c < d.length; ++c) -1 === d[c] && (d[c] = ++t); for (i = c = 0; c < b; ++c) j = f[c], j === "(" ? (++i, d[i] === void 0 && (f[c] = "(?:")) : "\\" === j.charAt(0) &&
            (j = +j.substring(1)) && j <= i && (f[c] = "\\" + d[i]); for (i = c = 0; c < b; ++c) "^" === f[c] && "^" !== f[c + 1] && (f[c] = ""); if (a.ignoreCase && s) for (c = 0; c < b; ++c) j = f[c], a = j.charAt(0), j.length >= 2 && a === "[" ? f[c] = h(j) : a !== "\\" && (f[c] = j.replace(/[A-Za-z]/g, function (a) { a = a.charCodeAt(0); return "[" + String.fromCharCode(a & -33, a | 32) + "]" })); return f.join("")
        } for (var t = 0, s = !1, l = !1, p = 0, d = a.length; p < d; ++p) { var g = a[p]; if (g.ignoreCase) l = !0; else if (/[a-z]/i.test(g.source.replace(/\\u[\da-f]{4}|\\x[\da-f]{2}|\\[^UXux]/gi, ""))) { s = !0; l = !1; break } } for (var r =
        { b: 8, t: 9, n: 10, v: 11, f: 12, r: 13 }, n = [], p = 0, d = a.length; p < d; ++p) { g = a[p]; if (g.global || g.multiline) throw Error("" + g); n.push("(?:" + y(g) + ")") } return RegExp(n.join("|"), l ? "gi" : "g")
    } function M(a) {
        function m(a) {
            switch (a.nodeType) {
                case 1: if (e.test(a.className)) break; for (var g = a.firstChild; g; g = g.nextSibling) m(g); g = a.nodeName; if ("BR" === g || "LI" === g) h[s] = "\n", t[s << 1] = y++, t[s++ << 1 | 1] = a; break; case 3: case 4: g = a.nodeValue, g.length && (g = p ? g.replace(/\r\n?/g, "\n") : g.replace(/[\t\n\r ]+/g, " "), h[s] = g, t[s << 1] = y, y += g.length,
                t[s++ << 1 | 1] = a)
            }
        } var e = /(?:^|\s)nocode(?:\s|$)/, h = [], y = 0, t = [], s = 0, l; a.currentStyle ? l = a.currentStyle.whiteSpace : window.getComputedStyle && (l = document.defaultView.getComputedStyle(a, q).getPropertyValue("white-space")); var p = l && "pre" === l.substring(0, 3); m(a); return { a: h.join("").replace(/\n$/, ""), c: t }
    } function B(a, m, e, h) { m && (a = { a: m, d: a }, e(a), h.push.apply(h, a.e)) } function x(a, m) {
        function e(a) {
            for (var l = a.d, p = [l, "pln"], d = 0, g = a.a.match(y) || [], r = {}, n = 0, z = g.length; n < z; ++n) {
                var f = g[n], b = r[f], o = void 0, c; if (typeof b ===
                "string") c = !1; else { var i = h[f.charAt(0)]; if (i) o = f.match(i[1]), b = i[0]; else { for (c = 0; c < t; ++c) if (i = m[c], o = f.match(i[1])) { b = i[0]; break } o || (b = "pln") } if ((c = b.length >= 5 && "lang-" === b.substring(0, 5)) && !(o && typeof o[1] === "string")) c = !1, b = "src"; c || (r[f] = b) } i = d; d += f.length; if (c) { c = o[1]; var j = f.indexOf(c), k = j + c.length; o[2] && (k = f.length - o[2].length, j = k - c.length); b = b.substring(5); B(l + i, f.substring(0, j), e, p); B(l + i + j, c, C(b, c), p); B(l + i + k, f.substring(k), e, p) } else p.push(l + i, b)
            } a.e = p
        } var h = {}, y; (function () {
            for (var e = a.concat(m),
            l = [], p = {}, d = 0, g = e.length; d < g; ++d) { var r = e[d], n = r[3]; if (n) for (var k = n.length; --k >= 0;) h[n.charAt(k)] = r; r = r[1]; n = "" + r; p.hasOwnProperty(n) || (l.push(r), p[n] = q) } l.push(/[\S\s]/); y = L(l)
        })(); var t = m.length; return e
    } function u(a) {
        var m = [], e = []; a.tripleQuotedStrings ? m.push(["str", /^(?:'''(?:[^'\\]|\\[\S\s]|''?(?=[^']))*(?:'''|$)|"""(?:[^"\\]|\\[\S\s]|""?(?=[^"]))*(?:"""|$)|'(?:[^'\\]|\\[\S\s])*(?:'|$)|"(?:[^"\\]|\\[\S\s])*(?:"|$))/, q, "'\""]) : a.multiLineStrings ? m.push(["str", /^(?:'(?:[^'\\]|\\[\S\s])*(?:'|$)|"(?:[^"\\]|\\[\S\s])*(?:"|$)|`(?:[^\\`]|\\[\S\s])*(?:`|$))/,
        q, "'\"`"]) : m.push(["str", /^(?:'(?:[^\n\r'\\]|\\.)*(?:'|$)|"(?:[^\n\r"\\]|\\.)*(?:"|$))/, q, "\"'"]); a.verbatimStrings && e.push(["str", /^@"(?:[^"]|"")*(?:"|$)/, q]); var h = a.hashComments; h && (a.cStyleComments ? (h > 1 ? m.push(["com", /^#(?:##(?:[^#]|#(?!##))*(?:###|$)|.*)/, q, "#"]) : m.push(["com", /^#(?:(?:define|elif|else|endif|error|ifdef|include|ifndef|line|pragma|undef|warning)\b|[^\n\r]*)/, q, "#"]), e.push(["str", /^<(?:(?:(?:\.\.\/)*|\/?)(?:[\w-]+(?:\/[\w-]+)+)?[\w-]+\.h|[a-z]\w*)>/, q])) : m.push(["com", /^#[^\n\r]*/,
        q, "#"])); a.cStyleComments && (e.push(["com", /^\/\/[^\n\r]*/, q]), e.push(["com", /^\/\*[\S\s]*?(?:\*\/|$)/, q])); a.regexLiterals && e.push(["lang-regex", /^(?:^^\.?|[!+-]|!=|!==|#|%|%=|&|&&|&&=|&=|\(|\*|\*=|\+=|,|-=|->|\/|\/=|:|::|;|<|<<|<<=|<=|=|==|===|>|>=|>>|>>=|>>>|>>>=|[?@[^]|\^=|\^\^|\^\^=|{|\||\|=|\|\||\|\|=|~|break|case|continue|delete|do|else|finally|instanceof|return|throw|try|typeof)\s*(\/(?=[^*/])(?:[^/[\\]|\\[\S\s]|\[(?:[^\\\]]|\\[\S\s])*(?:]|$))+\/)/]); (h = a.types) && e.push(["typ", h]); a = ("" + a.keywords).replace(/^ | $/g,
        ""); a.length && e.push(["kwd", RegExp("^(?:" + a.replace(/[\s,]+/g, "|") + ")\\b"), q]); m.push(["pln", /^\s+/, q, " \r\n\t\xa0"]); e.push(["lit", /^@[$_a-z][\w$@]*/i, q], ["typ", /^(?:[@_]?[A-Z]+[a-z][\w$@]*|\w+_t\b)/, q], ["pln", /^[$_a-z][\w$@]*/i, q], ["lit", /^(?:0x[\da-f]+|(?:\d(?:_\d+)*\d*(?:\.\d*)?|\.\d\+)(?:e[+-]?\d+)?)[a-z]*/i, q, "0123456789"], ["pln", /^\\[\S\s]?/, q], ["pun", /^.[^\s\w"-$'./@\\`]*/, q]); return x(m, e)
    } function D(a, m) {
        function e(a) {
            switch (a.nodeType) {
                case 1: if (k.test(a.className)) break; if ("BR" === a.nodeName) h(a),
                a.parentNode && a.parentNode.removeChild(a); else for (a = a.firstChild; a; a = a.nextSibling) e(a); break; case 3: case 4: if (p) { var b = a.nodeValue, d = b.match(t); if (d) { var c = b.substring(0, d.index); a.nodeValue = c; (b = b.substring(d.index + d[0].length)) && a.parentNode.insertBefore(s.createTextNode(b), a.nextSibling); h(a); c || a.parentNode.removeChild(a) } }
            }
        } function h(a) {
            function b(a, d) { var e = d ? a.cloneNode(!1) : a, f = a.parentNode; if (f) { var f = b(f, 1), g = a.nextSibling; f.appendChild(e); for (var h = g; h; h = g) g = h.nextSibling, f.appendChild(h) } return e }
            for (; !a.nextSibling;) if (a = a.parentNode, !a) return; for (var a = b(a.nextSibling, 0), e; (e = a.parentNode) && e.nodeType === 1;) a = e; d.push(a)
        } var k = /(?:^|\s)nocode(?:\s|$)/, t = /\r\n?|\n/, s = a.ownerDocument, l; a.currentStyle ? l = a.currentStyle.whiteSpace : window.getComputedStyle && (l = s.defaultView.getComputedStyle(a, q).getPropertyValue("white-space")); var p = l && "pre" === l.substring(0, 3); for (l = s.createElement("LI") ; a.firstChild;) l.appendChild(a.firstChild); for (var d = [l], g = 0; g < d.length; ++g) e(d[g]); m === (m | 0) && d[0].setAttribute("value",
        m); var r = s.createElement("OL"); r.className = "linenums"; for (var n = Math.max(0, m - 1 | 0) || 0, g = 0, z = d.length; g < z; ++g) l = d[g], l.className = "L" + (g + n) % 10, l.firstChild || l.appendChild(s.createTextNode("\xa0")), r.appendChild(l); a.appendChild(r)
    } function k(a, m) { for (var e = m.length; --e >= 0;) { var h = m[e]; A.hasOwnProperty(h) ? window.console && console.warn("cannot override language handler %s", h) : A[h] = a } } function C(a, m) { if (!a || !A.hasOwnProperty(a)) a = /^\s*</.test(m) ? "default-markup" : "default-code"; return A[a] } function E(a) {
        var m =
        a.g; try {
            var e = M(a.h), h = e.a; a.a = h; a.c = e.c; a.d = 0; C(m, h)(a); var k = /\bMSIE\b/.test(navigator.userAgent), m = /\n/g, t = a.a, s = t.length, e = 0, l = a.c, p = l.length, h = 0, d = a.e, g = d.length, a = 0; d[g] = s; var r, n; for (n = r = 0; n < g;) d[n] !== d[n + 2] ? (d[r++] = d[n++], d[r++] = d[n++]) : n += 2; g = r; for (n = r = 0; n < g;) { for (var z = d[n], f = d[n + 1], b = n + 2; b + 2 <= g && d[b + 1] === f;) b += 2; d[r++] = z; d[r++] = f; n = b } for (d.length = r; h < p;) {
                var o = l[h + 2] || s, c = d[a + 2] || s, b = Math.min(o, c), i = l[h + 1], j; if (i.nodeType !== 1 && (j = t.substring(e, b))) {
                    k && (j = j.replace(m, "\r")); i.nodeValue =
                    j; var u = i.ownerDocument, v = u.createElement("SPAN"); v.className = d[a + 1]; var x = i.parentNode; x.replaceChild(v, i); v.appendChild(i); e < o && (l[h + 1] = i = u.createTextNode(t.substring(b, o)), x.insertBefore(i, v.nextSibling))
                } e = b; e >= o && (h += 2); e >= c && (a += 2)
            }
        } catch (w) { "console" in window && console.log(w && w.stack ? w.stack : w) }
    } var v = ["break,continue,do,else,for,if,return,while"], w = [[v, "auto,case,char,const,default,double,enum,extern,float,goto,int,long,register,short,signed,sizeof,static,struct,switch,typedef,union,unsigned,void,volatile"],
    "catch,class,delete,false,import,new,operator,private,protected,public,this,throw,true,try,typeof"], F = [w, "alignof,align_union,asm,axiom,bool,concept,concept_map,const_cast,constexpr,decltype,dynamic_cast,explicit,export,friend,inline,late_check,mutable,namespace,nullptr,reinterpret_cast,static_assert,static_cast,template,typeid,typename,using,virtual,where"], G = [w, "abstract,boolean,byte,extends,final,finally,implements,import,instanceof,null,native,package,strictfp,super,synchronized,throws,transient"],
    H = [G, "as,base,by,checked,decimal,delegate,descending,dynamic,event,fixed,foreach,from,group,implicit,in,interface,internal,into,is,lock,object,out,override,orderby,params,partial,readonly,ref,sbyte,sealed,stackalloc,string,select,uint,ulong,unchecked,unsafe,ushort,var"], w = [w, "debugger,eval,export,function,get,null,set,undefined,var,with,Infinity,NaN"], I = [v, "and,as,assert,class,def,del,elif,except,exec,finally,from,global,import,in,is,lambda,nonlocal,not,or,pass,print,raise,try,with,yield,False,True,None"],
    J = [v, "alias,and,begin,case,class,def,defined,elsif,end,ensure,false,in,module,next,nil,not,or,redo,rescue,retry,self,super,then,true,undef,unless,until,when,yield,BEGIN,END"], v = [v, "case,done,elif,esac,eval,fi,function,in,local,set,then,until"], K = /^(DIR|FILE|vector|(de|priority_)?queue|list|stack|(const_)?iterator|(multi)?(set|map)|bitset|u?(int|float)\d*)/, N = /\S/, O = u({
        keywords: [F, H, w, "caller,delete,die,do,dump,elsif,eval,exit,foreach,for,goto,if,import,last,local,my,next,no,our,print,package,redo,require,sub,undef,unless,until,use,wantarray,while,BEGIN,END" +
        I, J, v], hashComments: !0, cStyleComments: !0, multiLineStrings: !0, regexLiterals: !0
    }), A = {}; k(O, ["default-code"]); k(x([], [["pln", /^[^<?]+/], ["dec", /^<!\w[^>]*(?:>|$)/], ["com", /^<\!--[\S\s]*?(?:--\>|$)/], ["lang-", /^<\?([\S\s]+?)(?:\?>|$)/], ["lang-", /^<%([\S\s]+?)(?:%>|$)/], ["pun", /^(?:<[%?]|[%?]>)/], ["lang-", /^<xmp\b[^>]*>([\S\s]+?)<\/xmp\b[^>]*>/i], ["lang-js", /^<script\b[^>]*>([\S\s]*?)(<\/script\b[^>]*>)/i], ["lang-css", /^<style\b[^>]*>([\S\s]*?)(<\/style\b[^>]*>)/i], ["lang-in.tag", /^(<\/?[a-z][^<>]*>)/i]]),
    ["default-markup", "htm", "html", "mxml", "xhtml", "xml", "xsl"]); k(x([["pln", /^\s+/, q, " \t\r\n"], ["atv", /^(?:"[^"]*"?|'[^']*'?)/, q, "\"'"]], [["tag", /^^<\/?[a-z](?:[\w-.:]*\w)?|\/?>$/i], ["atn", /^(?!style[\s=]|on)[a-z](?:[\w:-]*\w)?/i], ["lang-uq.val", /^=\s*([^\s"'>]*(?:[^\s"'/>]|\/(?=\s)))/], ["pun", /^[/<->]+/], ["lang-js", /^on\w+\s*=\s*"([^"]+)"/i], ["lang-js", /^on\w+\s*=\s*'([^']+)'/i], ["lang-js", /^on\w+\s*=\s*([^\s"'>]+)/i], ["lang-css", /^style\s*=\s*"([^"]+)"/i], ["lang-css", /^style\s*=\s*'([^']+)'/i], ["lang-css",
    /^style\s*=\s*([^\s"'>]+)/i]]), ["in.tag"]); k(x([], [["atv", /^[\S\s]+/]]), ["uq.val"]); k(u({ keywords: F, hashComments: !0, cStyleComments: !0, types: K }), ["c", "cc", "cpp", "cxx", "cyc", "m"]); k(u({ keywords: "null,true,false" }), ["json"]); k(u({ keywords: H, hashComments: !0, cStyleComments: !0, verbatimStrings: !0, types: K }), ["cs"]); k(u({ keywords: G, cStyleComments: !0 }), ["java"]); k(u({ keywords: v, hashComments: !0, multiLineStrings: !0 }), ["bsh", "csh", "sh"]); k(u({ keywords: I, hashComments: !0, multiLineStrings: !0, tripleQuotedStrings: !0 }),
    ["cv", "py"]); k(u({ keywords: "caller,delete,die,do,dump,elsif,eval,exit,foreach,for,goto,if,import,last,local,my,next,no,our,print,package,redo,require,sub,undef,unless,until,use,wantarray,while,BEGIN,END", hashComments: !0, multiLineStrings: !0, regexLiterals: !0 }), ["perl", "pl", "pm"]); k(u({ keywords: J, hashComments: !0, multiLineStrings: !0, regexLiterals: !0 }), ["rb"]); k(u({ keywords: w, cStyleComments: !0, regexLiterals: !0 }), ["js"]); k(u({
        keywords: "all,and,by,catch,class,else,extends,false,finally,for,if,in,is,isnt,loop,new,no,not,null,of,off,on,or,return,super,then,true,try,unless,until,when,while,yes",
        hashComments: 3, cStyleComments: !0, multilineStrings: !0, tripleQuotedStrings: !0, regexLiterals: !0
    }), ["coffee"]); k(x([], [["str", /^[\S\s]+/]]), ["regex"]); window.prettyPrintOne = function (a, m, e) { var h = document.createElement("PRE"); h.innerHTML = a; e && D(h, e); E({ g: m, i: e, h: h }); return h.innerHTML }; window.prettyPrint = function (a) {
        function m() {
            for (var e = window.PR_SHOULD_USE_CONTINUATION ? l.now() + 250 : Infinity; p < h.length && l.now() < e; p++) {
                var n = h[p], k = n.className; if (k.indexOf("prettyprint") >= 0) {
                    var k = k.match(g), f, b; if (b =
                    !k) { b = n; for (var o = void 0, c = b.firstChild; c; c = c.nextSibling) var i = c.nodeType, o = i === 1 ? o ? b : c : i === 3 ? N.test(c.nodeValue) ? b : o : o; b = (f = o === b ? void 0 : o) && "CODE" === f.tagName } b && (k = f.className.match(g)); k && (k = k[1]); b = !1; for (o = n.parentNode; o; o = o.parentNode) if ((o.tagName === "pre" || o.tagName === "code" || o.tagName === "xmp") && o.className && o.className.indexOf("prettyprint") >= 0) { b = !0; break } b || ((b = (b = n.className.match(/\blinenums\b(?::(\d+))?/)) ? b[1] && b[1].length ? +b[1] : !0 : !1) && D(n, b), d = { g: k, h: n, i: b }, E(d))
                }
            } p < h.length ? setTimeout(m,
            250) : a && a()
        } for (var e = [document.getElementsByTagName("pre"), document.getElementsByTagName("code"), document.getElementsByTagName("xmp")], h = [], k = 0; k < e.length; ++k) for (var t = 0, s = e[k].length; t < s; ++t) h.push(e[k][t]); var e = q, l = Date; l.now || (l = { now: function () { return +new Date } }); var p = 0, d, g = /\blang(?:uage)?-([\w.]+)(?!\S)/; m()
    }; window.PR = {
        createSimpleLexer: x, registerLangHandler: k, sourceDecorator: u, PR_ATTRIB_NAME: "atn", PR_ATTRIB_VALUE: "atv", PR_COMMENT: "com", PR_DECLARATION: "dec", PR_KEYWORD: "kwd", PR_LITERAL: "lit",
        PR_NOCODE: "nocode", PR_PLAIN: "pln", PR_PUNCTUATION: "pun", PR_SOURCE: "src", PR_STRING: "str", PR_TAG: "tag", PR_TYPE: "typ"
    }
})();

/*
 HTML5 Shiv v3.6.2pre | @afarkas @jdalton @jon_neal @rem | MIT/GPL2 Licensed
 Uncompressed source: https://github.com/aFarkas/html5shiv
*/
(function (l, f) {
    function m() { var a = e.elements; return "string" == typeof a ? a.split(" ") : a } function i(a) { var b = n[a[o]]; b || (b = {}, h++, a[o] = h, n[h] = b); return b } function p(a, b, c) { b || (b = f); if (g) return b.createElement(a); c || (c = i(b)); b = c.cache[a] ? c.cache[a].cloneNode() : r.test(a) ? (c.cache[a] = c.createElem(a)).cloneNode() : c.createElem(a); return b.canHaveChildren && !s.test(a) ? c.frag.appendChild(b) : b } function t(a, b) {
        if (!b.cache) b.cache = {}, b.createElem = a.createElement, b.createFrag = a.createDocumentFragment, b.frag = b.createFrag();
        a.createElement = function (c) { return !e.shivMethods ? b.createElem(c) : p(c, a, b) }; a.createDocumentFragment = Function("h,f", "return function(){var n=f.cloneNode(),c=n.createElement;h.shivMethods&&(" + m().join().replace(/\w+/g, function (a) { b.createElem(a); b.frag.createElement(a); return 'c("' + a + '")' }) + ");return n}")(e, b.frag)
    } function q(a) {
        a || (a = f); var b = i(a); if (e.shivCSS && !j && !b.hasCSS) {
            var c, d = a; c = d.createElement("p"); d = d.getElementsByTagName("head")[0] || d.documentElement; c.innerHTML = "x<style>article,aside,figcaption,figure,footer,header,hgroup,main,nav,section{display:block}mark{background:#FF0;color:#000}</style>";
            c = d.insertBefore(c.lastChild, d.firstChild); b.hasCSS = !!c
        } g || t(a, b); return a
    } var k = l.html5 || {}, s = /^<|^(?:button|map|select|textarea|object|iframe|option|optgroup)$/i, r = /^(?:a|b|code|div|fieldset|h1|h2|h3|h4|h5|h6|i|label|li|ol|p|q|span|strong|style|table|tbody|td|th|tr|ul)$/i, j, o = "_html5shiv", h = 0, n = {}, g; (function () {
        try {
            var a = f.createElement("a"); a.innerHTML = "<xyz></xyz>"; j = "hidden" in a; var b; if (!(b = 1 == a.childNodes.length)) {
                f.createElement("a"); var c = f.createDocumentFragment(); b = "undefined" == typeof c.cloneNode ||
                "undefined" == typeof c.createDocumentFragment || "undefined" == typeof c.createElement
            } g = b
        } catch (d) { g = j = !0 }
    })(); var e = {
        elements: k.elements || "abbr article aside audio bdi canvas data datalist details figcaption figure footer header hgroup main mark meter nav output progress section summary time video", version: "3.6.2pre", shivCSS: !1 !== k.shivCSS, supportsUnknownElements: g, shivMethods: !1 !== k.shivMethods, type: "default", shivDocument: q, createElement: p, createDocumentFragment: function (a, b) {
            a || (a = f); if (g) return a.createDocumentFragment();
            for (var b = b || i(a), c = b.frag.cloneNode(), d = 0, e = m(), h = e.length; d < h; d++) c.createElement(e[d]); return c
        }
    }; l.html5 = e; q(f)
})(this, document);

/*
 bootstrap dropdown hover show
*/
; (function ($, window, undefined) {
    // outside the scope of the jQuery plugin to
    // keep track of all dropdowns
    var $allDropdowns = $();

    // if instantlyCloseOthers is true, then it will instantly
    // shut other nav items when a new one is hovered over
    $.fn.dropdownHover = function (options) {
        // don't do anything if touch is supported
        // (plugin causes some issues on mobile)
        if ('ontouchstart' in document) return this; // don't want to affect chaining

        // the element we really care about
        // is the dropdown-toggle's parent
        $allDropdowns = $allDropdowns.add(this.parent());

        return this.each(function () {
            var $this = $(this),
                $parent = $this.parent(),
                defaults = {
                    delay: 500,
                    hoverDelay: 0,
                    instantlyCloseOthers: true
                },
                data = {
                    delay: $(this).data('delay'),
                    hoverDelay: $(this).data('hover-delay'),
                    instantlyCloseOthers: $(this).data('close-others')
                },
                showEvent = 'show.bs.dropdown',
                hideEvent = 'hide.bs.dropdown',
                // shownEvent  = 'shown.bs.dropdown',
                // hiddenEvent = 'hidden.bs.dropdown',
                settings = $.extend(true, {}, defaults, options, data),
                timeout, timeoutHover;

            $parent.hover(function (event) {
                // so a neighbor can't open the dropdown
                //if (!$parent.hasClass('open') && !$this.is(event.target)) {
                //    // stop this event, stop executing any code
                //    // in this callback but continue to propagate
                //    return true;
                //}
                openDropdown(event);
            }, function () {
                // clear timer for hover event
                window.clearTimeout(timeoutHover)
                timeout = window.setTimeout(function () {
                    $this.attr('aria-expanded', 'false');
                    $parent.removeClass('open');
                    $this.trigger(hideEvent);
                }, settings.delay);
            });

            // this helps with button groups!
            $this.hover(function (event) {
                // this helps prevent a double event from firing.
                // see https://github.com/CWSpear/bootstrap-hover-dropdown/issues/55
                if (!$parent.hasClass('open') && !$parent.is(event.target)) {
                    // stop this event, stop executing any code
                    // in this callback but continue to propagate
                    return true;
                }

                openDropdown(event);
            });

            // handle submenus
            $parent.find('.dropdown-submenu').each(function () {
                var $this = $(this);
                var subTimeout;
                $this.hover(function () {
                    window.clearTimeout(subTimeout);
                    $this.children('.dropdown-menu').show();
                    // always close submenu siblings instantly
                    $this.siblings().children('.dropdown-menu').hide();
                }, function () {
                    var $submenu = $this.children('.dropdown-menu');
                    subTimeout = window.setTimeout(function () {
                        $submenu.hide();
                    }, settings.delay);
                });
            });

            function openDropdown(event) {
                // clear dropdown timeout here so it doesnt close before it should
                window.clearTimeout(timeout);
                // restart hover timer
                window.clearTimeout(timeoutHover);

                // delay for hover event.  
                timeoutHover = window.setTimeout(function () {
                    $allDropdowns.find(':focus').blur();

                    if (settings.instantlyCloseOthers === true)
                        $allDropdowns.removeClass('open');

                    // clear timer for hover event
                    window.clearTimeout(timeoutHover);
                    $this.attr('aria-expanded', 'true');
                    $parent.addClass('open');
                    $this.trigger(showEvent);
                }, settings.hoverDelay);
            }
        });
    };

    $(document).ready(function () {
        // apply dropdownHover to all elements with the data-hover="dropdown" attribute
        $('[data-hover="dropdown"]').dropdownHover();
    });
})(jQuery, window);
