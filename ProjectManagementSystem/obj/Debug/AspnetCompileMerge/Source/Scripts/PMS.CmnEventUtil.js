
// Set position of button search & clear
function setButtonSearch() {
    if ($('.sidebar-collapse').length > 0) {
        $('div.search-action #btnClear').css('left', $(window).width() - 182);
        $('div.search-action #btnSearch').css('left', $(window).width() - 282);
        $('div.search-action #btnSearchBudget').css('left', $(window).width() - 282-30);

        $('div.show-hide-search').css('left', $(window).width() - 97);
    } else {
        $('div.search-action #btnClear').css('left', $(window).width() - 180 - 130);
        $('div.search-action #btnSearch').css('left', $(window).width() - 280 - 130);
        $('div.search-action #btnSearchBudget').css('left', $(window).width() - 280 - 130 - 30);

        $('div.show-hide-search').css('left', $(window).width() - 97);
    }

    if (!window.screenTop && !window.screenY && !!navigator.userAgent.match(/version\/11(\.[0-9]*)? safari/i)) { // check if browser is Safari version 11 in Full screen mode
        if ($(window).width() == 1582) {
            if ($('.sidebar-collapse').length > 0) {
                $('div.search-action #btnClear').css('left', $(window).width() - 182 - 77);
                $('div.search-action #btnSearch').css('left', $(window).width() - 282 - 77);
                $('div.search-action #btnSearchBudget').css('left', $(window).width() - 282 - 30 - 77);

                $('div.show-hide-search').css('left', $(window).width() - 97 - 77);
            } else {
                $('div.search-action #btnClear').css('left', $(window).width() - 180 - 130 - 77);
                $('div.search-action #btnSearch').css('left', $(window).width() - 280 - 130 - 77);
                $('div.search-action #btnSearchBudget').css('left', $(window).width() - 280 - 130 - 30 - 77);

                $('div.show-hide-search').css('left', $(window).width() - 97 - 77);
            }
        }
    }
    var isChrome = !!window.chrome && !!window.chrome.webstore;
    if (window.location.pathname.indexOf('PMS09002') != -1 && $(window).width() > 1150 && isChrome && $('body').hasClass('sidebar-collapse') === false) {
        $('.col-left').css('margin-right', -2);
    }
    else {
        $('.col-left').css('margin-right', '');
    }
}

$(document).ready(function () {
    if ($('#companyImage').attr('src') === '') {
        $('#companyImage').css('display', 'none');
    }

    window.onload = function () {
        $('#onloadDiv').hide();
    }

    setButtonSearch();

    window.addEventListener('resize', function (event) {
        setButtonSearch();
    });

    PMS.utility.validFullHalfSize($(".date > input, .display-date, .datefield"));
    PMS.utility.imeControl($(".date > input, .display-date"), 'disable');
});

function SetEventBeforeUnload() {
    window.onbeforeunload = function (e) {
        $.ajax({
            type: "GET",
            url: '/Common/ClearSearchResult',
            data: {
                TAB_ID: $('#tabId').val()
            },
            dataType: 'json',
            async: false,
            cache: false,
            success: function (result) {
            },
            error: function (err) {
            }
        });
    };
}

// Event auto start on load
$(function () {

    //Disable event onBeforeUnload when submit form to make sure session is not cleared
    $(".frmExport").submit(function (e) {
        e.preventDefault();
        window.onbeforeunload = null;
        this.submit();
        setTimeout(function () {
            SetEventBeforeUnload();
        }, 10);
    });

    // Active current node on left menu
    if (window.location.pathname !== '/') {
        $(".sidebar-small-menu, .main-sidebar").find("li").children("a").each(function () {
            var url = $(this).attr("href");
            if (url.indexOf(window.location.pathname) > -1) {
                $(this).parents('.parent-node').addClass("active");
            }
        });
        $(".sidebar-small-menu, .main-sidebar").find("li").children("div").children("a").each(function () {
            var url = $(this).attr("href");
            if (url.indexOf(window.location.pathname) > -1) {
                $(this).parents('.parent-node').addClass("active");
            }
        });
    }

    // Bind event for calendar
    $(".date.datepicker-months").datepicker({
        format: "yyyy/mm",
        viewMode: "months",
        minViewMode: "months",
        autoclose: true,
        language: 'ja'
    });
    $('.date.datepicker-months input').off('focus');

    $(".date.datepicker-days").datepicker({
        autoclose: true,
        language: 'ja'
    });
    $('.date.datepicker-days input').off('focus');

    // Bind event for calendar
    $(".date.datepicker-years").datepicker({
        format: "yyyy",
        viewMode: "years",
        minViewMode: "years",
        autoclose: true,
        language: 'ja'
    });
    $('.date.datepicker-months input').off('focus');
});

// Event show hide search form
$(document).on('click', '.show-hide-search', function (e) {
    $('.search-form').stop(true).slideToggle('fast');

    var i_first = $(this).children(":first");
    var i_last = $(this).children(":last");

    if (i_first.hasClass('hide')) {
        i_last.addClass('hide');
        i_first.removeClass('hide');

        setTimeout(function () {
            $('div.search-action button').hide();
        }, 50);
    } else {
        i_first.addClass('hide');
        i_last.removeClass('hide');

        setTimeout(function () {
            $('div.search-action button').show();
        }, 100);
    }
});

// Event show hide menu-left
$(document).off('a.sidebar-toggle i');
$(document).on('click', 'a.sidebar-toggle i', function () {
    if ($('body').hasClass('sidebar-collapse') === true) {
        localStorage.setItem('status-menu', 'close');
    } else {
        localStorage.setItem('status-menu', 'open');
    }

    setButtonSearch();
    setTimeout(function () {
        if ($(".actual-work-regist-new .div-right").length > 0) {
            $(".actual-work-regist-new .div-right").vgrid({
                easing: "easeOutQuint",
                time: 400,
                delay: 20,
                wait: 500
            });
        }
        if ($('#total-time .total-time-left').length > 0) {
            $('#total-time .total-time-left').width($('.actual-work-regist-new .div-left').outerWidth());
        }
    }, 300);
});

// Event show hide menu-left
$(document).off('.log-out');
$(document).on('click', '.log-out', function () {
    localStorage.setItem('status-menu', null);
});

// control input on calendar
$(document).off('.date > input, .display-date');
$(document).on('keydown', '.date > input, .display-date', function (e) {
    var charCode = event.keyCode;
    var handled = true;

    if (((charCode > 47 && 58 > charCode) || (charCode > 95 && 106 > charCode)) && !event.shiftKey) {
        handled = false;
    } else {
        switch (charCode) {
            case 8: // backspace
                handled = false;
                break;
            case 9: // tab
                handled = false;
                break;
            case 13: // enter
                handled = false;
                break;
            case 35: // end
                handled = false;
                break;
            case 36: // home
                handled = false;
                break;
            case 37: // left
                handled = false;
                break;
            case 39: // right
                handled = false;
                break;
            case 46: // delete
                handled = false;
                break;
            case 111: // char '/' on digital key
                handled = false;
                break;
            case 191: // char '/'
                if (!event.shiftKey) {
                    handled = false;
                } else {
                    handled = true;
                }
                break;
            case 116: // f5
                handled = false;
                break;
             case 65: // ctrl + a
                if (event.ctrlKey || event.metaKey) {
                    handled = false;
                    break;
                }
            case 67: // ctrl + c
                if (event.ctrlKey || event.metaKey) {
                    handled = false;
                    break;
                }
            case 86: // ctrl + v
                if (event.ctrlKey || event.metaKey) {
                    handled = false;
                    break;
                }
        }
    }

    if (handled) {
        event.preventDefault();
        event.stopPropagation();
    }
});
