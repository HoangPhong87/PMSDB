/*!
 * File: PMS.Utility.js
 * Company: i-Enter Asia
 * Copyright © 2014 i-Enter Asia. All rights reserved.
 * Project: Project Management System
 * Author: HoangPS
 * Created date: 2014/06/02
 */

var PMS = PMS || {};

PMS.utility = (function () {
    var Constant = {
        MIN_YEAR: 1753,
        MAX_YEAR: 9999,
        ERR_FORMAT: 'の入力形式に誤りがあります。',
        DATE_FORMAT: 'yyyy/mm/dd',
        URL_REDIRECT_TIMEOUT: '/PMS01001/Login',
        ERR_CONNECT_TIMEOUT: '接続の有効時間が切れました。再度ログインしなおしてください。',
        ERR_OUT_OF_DATE_LICENSE: '契約プランの期限が切れているか無効になっています。',
        REGX: /[^a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\_\^\@@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]/g,
        DIALOG_TYPE: {
            INFORMATION: "<i class='fa fa-check-circle'></i>",
            WARNING: "<i class='fa fa-warning'></i>"
        },
        MA_SALES_HEADER: '売上 / 予定原価'
    }

    // Format Time: decimal number to 2 digits
    function formatMinute(x) {
        if (x == "" || x == 0)
            return "00";
        else if (x < 10)
            return '0' + x;
        else
            return '' + x;
    }

    // Format Time: decimal number to 2 digits
    function formatHour(x) {
        if (x == "" || x == 0)
            return "00";
        else if (x >= 10 || x <= -10) {
            return x;
        } else if (x < 10 && x > 0) {
            return '0' + x;
        } else if (x < 0 && x > -10) {
            return '-0' + x * (-1);
        }
    }

    // Format money in textbox to string with symbol ','
    function formatMoney(obj) {
        obj = obj != null ? obj : '.money';

        $(obj).each(function () {
            var data = convertMoneyToInt($(this).val());
            var money = convertIntToMoney(data);

            $(this).val(money);
        });
    }

    // Format money in label to string with symbol ','
    function formatMoneyLabel() {
        $('label.lbl-money, label.money').each(function () {
            var data = convertMoneyToInt($(this).text(), true);
            var money = convertIntToMoney(data);

            $(this).text(money);
        });
    }

    // focus textbox
    function focusTextbox() {
        $(document).off('input:text, textarea, input:password');
        $(document).on('focus', 'input:text, textarea, input:password', function () {
            $(this).focus(function () { $(this).select(); })
            $(this).mouseup(function (e) {
                e.preventDefault();
                $(this).unbind(e.type);
            });
        });
    }

    // Format money string to integer
    function convertMoneyToInt(input, canNegative) {
        var strValue = input.replace(/,/g, '');
        var intValue = validPositiveNumeric(strValue) ? parseInt(strValue) : 0;

        if (typeof (canNegative) !== "undefined" && canNegative == true)
            intValue = validNegativeNumeric(strValue) ? parseInt(strValue) : 0;

        return intValue;
    }

    // Format integer to money string with symbol ','
    function convertIntToMoney(input) {
        return input.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    }

    // Replace all symbol ',' in money
    function replaceAllMoney() {
        $('.money').each(function () {
            var data = $(this).val();
            if (data.length > 0) {
                $(this).val(data.replace(/,/g, ''));
            }
        });
    }

    // Check isvalid date
    // Return true if valid, false if invalid
    function isValidDate(date) {
        var bits = date.split('/');
        var d = new Date(bits[0], bits[1] - 1, bits[2]);
        return d && (d.getMonth() + 1) == bits[1] && d.getDate() == Number(bits[2]);
    }

    // Check validation of date field
    // date is input data
    // format is datetime type (yyyy/mm/dd or yyyy/mm)
    // control is field name
    // return invalid message if invalid, null if valid
    function validDate(date, format, control) {
        if (date.length > 0) {
            if (format == 'yyyy/mm') {
                date += '/01';
            }

            if (!/^[0-9]{4}\/[0-9]{1,2}\/[0-9]{1,2}/.test(date) || !isValidDate(date)) {
                return (control + Constant.ERR_FORMAT);
            }

            var year = parseInt(date.split('/')[0]);

            if (Constant.MIN_YEAR > year || year > Constant.MAX_YEAR) {
                if (format == 'yyyy/mm') {
                    return control + 'は「1753/01」～「9999/12」の範囲で入力してください。';
                }
                return control + 'は「1753/01/01」～「9999/12/31」の範囲で入力してください。';
            }
        }

        return null;
    }

    // Compare startDate with endDate
    // Return true if valid, false if invalid
    function compareDate(startDate, endDate, format) {
        var start = $.datepicker.formatDate('yy/mm/dd', new Date(startDate));
        var end = $.datepicker.formatDate('yy/mm/dd', new Date(endDate));

        if (format == 'yyyy/mm') {
            start = $.datepicker.formatDate('yy/mm/dd', new Date(startDate + '/01'));
            end = $.datepicker.formatDate('yy/mm/dd', new Date(endDate + '/01'));
        }

        var valid = true;
        if (start > end) {
            valid = false;
        }

        return valid;
    }

    // Compare startDate with endDate
    // Return true if valid, false if invalid
    function compareDateRange(startDate, endDate, rangeMonth) {
        var start = new Date(startDate + '/01');
        var end = new Date(endDate + '/01');

        var valid = true;

        if ((end.getMonth() + end.getFullYear() * 12 - start.getMonth() - start.getFullYear() * 12) > rangeMonth) {
            valid = false;
        }

        return valid;
    }

    // Check range of a duration time
    function validateRangeYear(startYear, endYear, rangeYear) {
        var arrStart = startYear.split('/');
        var arrEnd = endYear.split('/');
        var sYear = parseInt(arrStart[0]);
        var sMonth = parseInt(arrStart[1]);
        var eYear = parseInt(arrEnd[0]);
        var eMonth = parseInt(arrEnd[1]);

        if ((eYear - sYear) * 12 + (eMonth - sMonth) > rangeYear) {
            return false;
        }
        return true;
    }

    // Check alphanumeric only
    function validAlphanumeric(input) {
        return /^[a-zA-Z0-9]*$/.test(input);
    }

    // Check positive number only
    function validPositiveNumeric(input) {
        if (input.indexOf('-') !== -1 || input.indexOf('.') !== -1 || !$.isNumeric(input)) {
            return false;
        }
        return true;
    }

    // Check negative number only
    function validNegativeNumeric(input) {
        if (input.indexOf('.') !== -1 || !$.isNumeric(input)) {
            return false;
        }
        return true;
    }

    // Check decimal number only
    function validDecimalNumeric(input) {
        if (input.indexOf('-') !== -1 || input.indexOf(',') !== -1 || !$.isNumeric(input)) {
            return false;
        }
        return true;
    }

    // Show client error message on the top of page
    function showClientError(errMessage, position) {
        position = typeof (position) === 'undefined' ? '#title' : position;

        $('.validation-summary-errors').remove();
        var html = '<div class="validation-summary-errors"><ul>';

        for (var i = 0; i < errMessage.length; i++) {
            html += '<li class="first last">' + errMessage[i] + '</li>';
        }
        html += '</ul></div>';
        $(position).after(html);
        $(document).scrollTop(0);
        $('.project-regist').scrollTop(0);

        return;
    }

    // Check user acount and password
    function validAcount(input) {
        var re = /^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$/;
        return re.test(input);
    }

    // Check numeric only
    function validNumeric(input) {
        return /^[0-9]+$/.test(input);
    }

    // Check phone number only
    function validPhone(input) {
        return /^[0-9/-]+$/.test(input);
    }

    // Check URL
    function validURL(input) {
        var re = /^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$/;
        return re.test(input);
    }

    // Check email
    function validEmail(input) {
        var re = /[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/;
        return re.test(input);
    }

    // Check valid IP address
    function validIpAddress(input) {
        var regeX = /^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/;
        return regeX.test(input);
    }

    //Check fullsize and half size
    function validFullHalfSize(control) {
        control.on("change keyup", function (e) {
            var text = $(this).val();
            var regX = /[^a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@@\[\;\:\]\,\.\/\`\{\+\*\}\>\?\<\'\?\/\\\_\ ]/g;
            if (regX.test(text)) {
                text = text.replace(regX, '');
                $(this).val(text);
            }
        });
    }

    //Check fullsize and half size
    function checkInputNumber(control) {
        control.on("change keyup", function (e) {
            var text = $(this).val();
            var regX = /[^0-9]/g;
            if (regX.test(text)) {
                $(this).val(text.replace(regX, ''));
            }
        });
    }

    // Control input IP address
    function checkInputIpAddress(control) {
        control.on("change keyup", function (e) {
            var text = $(this).val();
            var regX = /[^0-9.]/g;
            if (regX.test(text)) {
                $(this).val(text.replace(regX, ''));
            }
        });
    }

    function checkInputPhone(control) {
        control.on("change keyup", function (e) {
            var text = PMS.utility.Convert_JPCharacters(this.value, 'n');
            var regX = /[^0-9/-]/g;
            if (regX.test(text)) {
                $(this).val(text.replace(regX, ''));
            }
            if (text.indexOf('/') !== -1) {
                $(this).val(text.replace(/\//g, ''));
            }
        });

        control.on("blur", function (e) {
            var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
            $(this).val(value);
        });
    }

    // Encode html to string
    function htmlEncode(value) {
        return $('<div/>').text(value).html();
    }

    // Decode string to html
    function htmlDecode(value) {
        return $('<div/>').html(value).text();
    }

    // HTML encode by replace symbol " to code &quot;
    function htmlEncodeByReplace(value) {
        return value.replace(/"/g, "&quot;");
    }

    // Rounding decimal data
    function roundingDecimal(number, roundType, isPercent) {
        if (roundType == "")
            roundType = "03";

        if (isPercent) {
            switch (roundType) {
                case "01": // Round Down
                    number = number < 0 && number != Math.floor(number) ? (Math.floor(number * 10000) + 1) / 100 : Math.floor(number * 10000) / 100;
                    break;
                case "02": // Round Up
                    number = number > 0 && number != Math.floor(number) ? (Math.floor(number * 10000) + 1) / 100 : Math.floor(number * 10000) / 100;
                    break;
                case "03": // Round auto
                    var multiplier = 1;
                    if (number < 0)
                        multiplier = -1;
                    number = Math.round((number * 10000 * multiplier).toFixed(2)) / (100 * multiplier);
                    break;
            }
        } else {
            switch (roundType) {
                case "01": // Round Down
                    number = number < 0 && number != Math.floor(number) ? Math.floor(number) + 1 : Math.floor(number);
                    break;
                case "02": // Round Up
                    number = number > 0 && number != Math.floor(number) ? Math.floor(number) + 1 : Math.floor(number);
                    break;
                case "03": // Round auto
                    number = Math.round(number);
                    break;
            }
        }
        return number;
    };

    // Auto round percent value to decimal 2 digits after point
    function roundDecimalPercent(value) {
        var multiplier = 1;

        if (value < 0)
            multiplier = -1;

        value = Math.round((value * 10000 * multiplier).toFixed(2)) / (100 * multiplier);

        return value;
    }

    // Show message dialog
    function showMessageDialog(dialogType, message, urlRedirect, formId, callback) {
        BootstrapDialog.closeAll();

        BootstrapDialog.show({
            title: dialogType,
            message: message,
            closable: false,
            buttons: [{
                label: 'OK',
                hotkey: 13,
                cssClass: 'btn dark',
                action: function (dialog) {
                    if (typeof (formId) !== 'undefined' && formId !== null) {
                        $(formId).submit();

                        dialog.enableButtons(false);
                        dialog.getModalBody().html('<div class="loading"><img src="/Images/ajax-loader.gif"></div>');
                    }

                    if (typeof (urlRedirect) !== 'undefined' && urlRedirect !== null) {
                        window.location.href = urlRedirect;
                    }

                    if (typeof (callback) !== 'undefined') {
                        callback(null);
                    }
                    dialog.close();
                }
            }]
        });
    }

    // Show confirm dialog when submit form or redirect url
    function showSubmitConfirmDialog(message, formId, urlRedirect, callback) {
        BootstrapDialog.show({
            title: '<i class="fa fa-warning"></i>',
            message: message,
            closable: false,
            buttons: [{
                label: 'OK',
                hotkey: 13,
                cssClass: 'btn dark',
                action: function (dialog) {
                    if (typeof (formId) !== 'undefined' && formId !== null) {
                        PMS.utility.replaceAllMoney();

                        $(formId).submit();

                        dialog.enableButtons(false);
                        dialog.getModalBody().html('<div class="loading"><img src="/Images/ajax-loader.gif"></div>');
                    }

                    if (typeof (urlRedirect) !== 'undefined' && urlRedirect !== null) {
                        window.location.href = urlRedirect;
                    }

                    if (typeof (callback) !== 'undefined') {
                        callback(true);
                    }
                }
            }, {
                label: 'キャンセル',
                cssClass: 'btn light',
                action: function (dialog) {
                    dialog.close();

                    if (typeof (callback) !== 'undefined') {
                        callback(false);
                    }
                }
            }]
        });
    }

    // Check session timeout
    function IsAuthenticateTimeout(message, form) {
        var success = 1;
        $.ajax({
            type: "GET",
            url: '/Common/CheckTimeOut',
            dataType: 'json',
            async: false,
            cache: false,
            success: function (result) {
                if (form != '')
                    showSubmitConfirmDialog(message, form);

                success = 1;
                return;
            },
            error: function (err) {
                if (err.status == 419) { //419: Authentication Timeout
                    window.location.href = '/PMS01001/Login/timeout';
                } else if (err.status == 420) { // out of date license
                    window.location.href = '/ErrorOutOfDate';
                }
                else {
                    window.location.href = '/Error';
                }
                success = 0;
                return;
            }
        });
        return success;
    }

    // Check session timeout
    function IsAuthenticateTimeoutRedirect(message, link) {
        var success = 1;
        $.ajax({
            type: "GET",
            url: '/Common/CheckTimeOut',
            dataType: 'json',
            async: false,
            cache: false,
            success: function (result) {
                if (link != '')
                    showSubmitConfirmDialog(message, null, link);

                success = 1;
                return;
            },
            error: function (err) {
                if (err.status == 419) { //419: Authentication Timeout
                    window.location.href = '/PMS01001/Login/timeout';
                } else if (err.status == 420) { // out of date license
                    window.location.href = '/ErrorOutOfDate';
                }
                else {
                    window.location.href = '/Error';
                }

                success = 0;
                return;
            }
        });
        return success;
    }

    // Build html short text on data table
    function buildColumShortText(data, className) {
        data = data != null ? data : '';
        className = className != null ? className : '';

        var html = '<div data-overflow="no-dragon" class="short-text text-overflow ' + className + '" title="' + data + '">' + data + '</div>';
        return html;
    }

    // Build html link to detail on data table
    function builDetailFormCode(url, id) {
        var html = '<form method="POST" action="' + url + '">'
            + '<input type="hidden" name="id" value="' + id + '"/>'
            + '<a href="#" class="detail-link" onclick="$(this).parent().submit();">詳細</a>'
            + '</form>';
        return html;
    }

    // Control ime
    function imeControl(control, type) {
        if (type === 'active') {
            control.css('ime-mode', 'active');
        } else if (type === 'inactive') {
            control.css('ime-mode', 'inactive');
        } else {
            control.css('ime-mode', 'disabled');
        }
    }

    // Control ime
    function imeControlNew(control, type) {
        $(control).on({
            'focus': function () { $(this).attr('type', type); },
            'blur': function () { $(this).attr('type', 'text'); }
        });
    }

    // get data from server by Ajax GET. Return result
    function getDataByAjax(url, param) {
        var result;

        $.ajax({
            type: 'GET',
            url: url,
            data: param,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                result = data;
            },
            error: function (err) {
                if (err.status == 419) { //419: Authentication Timeout
                    window.location.href = '/PMS01001/Login/timeout';
                } else if (err.status == 420) { // out of date license
                    window.location.href = '/ErrorOutOfDate';
                }
                else {
                    window.location.href = '/Error';
                }

                return;
            }
        });
        return result;
    }

    // get data from server by Ajax GET with param is serialize to check valid data. Return result
    function checkDataExistByAjax(url, param) {
        var result;

        $.ajax({
            type: 'GET',
            url: url,
            data: param,
            dataType: 'json',
            traditional: true,
            async: false,
            cache: false,
            success: function (data) {
                result = data;
            },
            error: function (err) {
                if (err.status == 419) { //419: Authentication Timeout
                    window.location.href = '/PMS01001/Login/timeout';
                } else if (err.status == 420) { // out of date license
                    window.location.href = '/ErrorOutOfDate';
                }
                else {
                    window.location.href = '/Error';
                }

                return;
            }
        });
        return result;
    }

    // get image data by ajaxpace-active
    function getImageByAjax(url, param, callback) {
        $.ajax({
            type: 'GET',
            url: url,
            data: param,
            contentType: 'application/json',
            dataType: 'json',
            cache: false,
            success: function (data) {
                callback(data);
            },
            error: function (err) {
                if (err.status == 419) { //419: Authentication Timeout
                    window.location.href = '/PMS01001/Login/timeout';
                } else if (err.status == 420) { // out of date license
                    window.location.href = '/ErrorOutOfDate';
                }
                //else {
                //    window.location.href = '/Error';
                //}

                callback(null);
            }
        });
    }

    // Get month colum array on data table list bt start date and end date of project
    function getMonthCols(startDate, endDate) {
        var sY = parseInt(startDate.split('/')[0], 10);
        var sM = parseInt(startDate.split('/')[1], 10);
        var eY = parseInt(endDate.split('/')[0], 10);
        var eM = parseInt(endDate.split('/')[1], 10);
        var columArr = [];

        for (var Y = eY, M = eM, i = 0; Y > sY || (Y == sY && M >= sM); i++ , M--) {
            var YM;
            if (M == 0) {
                M = 12;
                Y--;
            }

            if (M < 10)
                YM = Y.toString() + '/0' + M.toString();
            else
                YM = Y.toString() + '/' + M.toString();
            columArr.unshift(YM);
        }
        return columArr;
    }

    // Set title colum with month for data table list
    function bindMonthCols(startDate, endDate) {
        var colums = getMonthCols(startDate, endDate);

        $('table.tb-detail tr th.th-month').remove();
        for (var i = 0; i < colums.length; i++) {
            $('table.tb-detail tr.month-colum').append('<th class="th-month">' + colums[i] + '</th>');
        }

        $('table.tb-ma-sales-center tr.month-colum th.th-month').each(function () {
            $(this).append('<br>' + Constant.MA_SALES_HEADER);
        });
    }

    function bindTotalCols(colums, startDate, endDate) {
        var htmlManday = '';
        var htmlMoney = '';
        var htmlMoneyForMaSales = '';

        if (colums == null)
            colums = getMonthCols(startDate, endDate);

        for (var i = 0; i < colums.length; i++) {
            htmlManday += '<td class="right">'
                + '<label class="font-normal" name="' + colums[i] + '">0.0</label>'
                + '<label>人日</label>'
                + '</td>';
            htmlMoney += '<td class="right" headers="' + colums[i] + '">'
                + '<label class="font-normal money" name="' + colums[i] + '">0</label>'
                + '<label>円</label>'
                + '</td>';

            htmlMoneyForMaSales += '<td class="right" headers="' + colums[i] + '">'
                + '<div class="div-sales"><div><label class="font-normal money ma-sales-by-month" name="' + colums[i] + '">0</label>'
                + '<label>円 /</label></div><div><label name= "' + colums[i] + '" class="font-normal plan-cost-by-month">0</label><label>円</label></div></div>'
                + '</td>';
        }

        $('table.tb-ma-center tr.tr-total, table.tb-ma-sales-center tr.tr-total, table.tb-sc-center tr.tr-total, table.tb-ovc-center tr.tr-total').empty();
        $('table.tb-ma-center tr.tr-total').append(htmlManday);
        $('table.tb-sc-center tr.tr-total, table.tb-ovc-center tr.tr-total').append(htmlMoney);
        $('table.tb-ma-sales-center tr.tr-total').append(htmlMoneyForMaSales);
    }

    // Bind data for tag link list by customer
    function bindTagsByCustomer(customerID, control) {
        var param = { customerId: customerID };
        var data = getDataByAjax('/Common/GetTagListJson', param);
        var $ddlTagLink = $(control);

        $ddlTagLink.empty();

        var html = '<option value="">指定なし</option>';

        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i].tag_id + '">' + htmlEncode(data[i].tag_name) + '</option>';
            }
        }

        $ddlTagLink.append(html);
    }

    // Validate control input data
    function validateControlInput(control) {
        control.on("change keyup", function (e) {
            var text = $(this).val();
            var regX
            if (Constant.REGX.test(text)) {
                text = text.replace(Constant.REGX, '');
                $(this).val(text);
            }
        });
    }

    // Remove all error validation on page
    function removeValidationError() {
        $('.validation-summary-errors').remove();
        $("label.label-validation-error").removeClass("label-validation-error");
    }

    // Check user permission to show button or link
    function checkPermission(control) {
        if ($(control).length > 0)
            return true

        return false;
    }

    // Check null and return value
    function nvl(targetString, defaultString) {
        var value = targetString;

        if (value == null) {
            value = defaultString;
        }

        return value;
    }

    function Convert_JPCharacters(str, option) {
        option = option || "KV";
        if (option.match('K')) {
            if (option.match('V')) {
                // han-kaku katakana (with daku-ten) to zen-kaku kata-kana
                str = str.replace(/([\uff66\uff73\uff76-\uff84\uff8a-\uff8e\uff9c])\uff9e/g, function (m, c) {
                    var f = {
                        0xff76: 0x30ac, 0xff77: 0x30ae, 0xff78: 0x30b0, 0xff79: 0x30b2, 0xff7a: 0x30b4,
                        0xff7b: 0x30b6, 0xff7c: 0x30b8, 0xff7d: 0x30ba, 0xff7e: 0x30bc, 0xff7f: 0x30be,
                        0xff80: 0x30c0, 0xff81: 0x30c2, 0xff82: 0x30c5, 0xff83: 0x30c7, 0xff84: 0x30c9,
                        0xff8a: 0x30d0, 0xff8b: 0x30d3, 0xff8c: 0x30d6, 0xff8d: 0x30d9, 0xff8e: 0x30dc,
                        0xff73: 0x30f4, 0xff9c: 0x30f7, 0xff66: 0x30fa
                    };
                    return String.fromCharCode(f[c.charCodeAt(c)]);
                })
                    .replace(/([\uff8a-\uff8e])\uff9f/g, function (m, c) {
                        var f = {
                            0xff8a: 0x30d1, 0xff8b: 0x30d4, 0xff8c: 0x30d7, 0xff8d: 0x30da, 0xff8e: 0x30dd
                        };
                        return String.fromCharCode(f[c.charCodeAt(c)]);
                    });
            }
            // han-kaku kata-kana to zen-kaku kata-kana
            str = str.replace(/[\uff61-\uff9d]/g, function (c) {
                var m = {
                    0xff61: 0x3002, 0xff62: 0x300c, 0xff63: 0x300d, 0xff64: 0x3001, 0xff65: 0x30fb,
                    0xff66: 0x30f2, 0xff67: 0x30a1, 0xff68: 0x30a3, 0xff69: 0x30a5, 0xff6a: 0x30a7,
                    0xff6b: 0x30a9, 0xff6c: 0x30e3, 0xff6d: 0x30e5, 0xff6e: 0x30e7, 0xff6f: 0x30c3,
                    0xff70: 0x30fc, 0xff71: 0x30a2, 0xff72: 0x30a4, 0xff73: 0x30a6, 0xff74: 0x30a8,
                    0xff75: 0x30aa, 0xff76: 0x30ab, 0xff77: 0x30ad, 0xff78: 0x30af, 0xff79: 0x30b1,
                    0xff7a: 0x30b3, 0xff7b: 0x30b5, 0xff7c: 0x30b7, 0xff7d: 0x30b9, 0xff7e: 0x30bb,
                    0xff7f: 0x30bd, 0xff80: 0x30bf, 0xff81: 0x30c1, 0xff82: 0x30c4, 0xff83: 0x30c6,
                    0xff84: 0x30c8, 0xff85: 0x30ca, 0xff86: 0x30cb, 0xff87: 0x30cc, 0xff88: 0x30cd,
                    0xff89: 0x30ce, 0xff8a: 0x30cf, 0xff8b: 0x30d2, 0xff8c: 0x30d5, 0xff8d: 0x30d8,
                    0xff8e: 0x30db, 0xff8f: 0x30de, 0xff90: 0x30df, 0xff91: 0x30e0, 0xff92: 0x30e1,
                    0xff93: 0x30e2, 0xff94: 0x30e4, 0xff95: 0x30e6, 0xff96: 0x30e8, 0xff97: 0x30e9,
                    0xff98: 0x30ea, 0xff99: 0x30eb, 0xff9a: 0x30ec, 0xff9b: 0x30ed, 0xff9c: 0x30ef,
                    0xff9d: 0x30f3
                };
                return String.fromCharCode(m[c.charCodeAt(0)]);
            });
        } else if (option.match('H')) {
            if (option.match('V')) {
                // han-kaku kata-kana (with daku-ten) to zen-kaku hira-gana
                str = str.replace(/([\uff73\uff76-\uff84\uff8a-\uff8e])\uff9e/g, function (m, c) {
                    var f = {
                        0xff73: 0x3094,
                        0xff76: 0x304c, 0xff77: 0x304e, 0xff78: 0x3050, 0xff79: 0x3052, 0xff7a: 0x3054,
                        0xff7b: 0x3056, 0xff7c: 0x3058, 0xff7d: 0x305a, 0xff7e: 0x305c, 0xff7f: 0x305e,
                        0xff80: 0x3060, 0xff81: 0x3062, 0xff82: 0x3065, 0xff83: 0x3067, 0xff84: 0x3069,
                        0xff8a: 0x3070, 0xff8b: 0x3073, 0xff8c: 0x3076, 0xff8d: 0x3079, 0xff8e: 0x307c
                        //0xff9c:0x30f7, 0xff66:0x30fa // vwa vwo
                    };
                    return String.fromCharCode(f[c.charCodeAt(c)]);
                })
                    .replace(/([\uff8a-\uff8e])\uff9f/g, function (m, c) {
                        var f = {
                            0xff8a: 0x3071, 0xff8b: 0x3074, 0xff8c: 0x3077, 0xff8d: 0x307a, 0xff8e: 0x307d
                        };
                        return String.fromCharCode(f[c.charCodeAt(c)]);
                    });
            }
            // han-kaku kata-kana to zen-kaku hira-gana
            str = str.replace(/[\uff61-\uff9d]/g, function (c) {
                var f = {
                    0xff61: 0x3002, 0xff62: 0x300c, 0xff63: 0x300d, 0xff64: 0x3001, 0xff65: 0x30fb,
                    0xff66: 0x3092, 0xff67: 0x3041, 0xff68: 0x3043, 0xff69: 0x3045, 0xff6a: 0x3047,
                    0xff6b: 0x3049, 0xff6c: 0x3083, 0xff6d: 0x3085, 0xff6e: 0x3087, 0xff6f: 0x3063,
                    0xff70: 0x30fc, 0xff71: 0x3042, 0xff72: 0x3044, 0xff73: 0x3046, 0xff74: 0x3048,
                    0xff75: 0x304a, 0xff76: 0x304b, 0xff77: 0x304d, 0xff78: 0x304f, 0xff79: 0x3051,
                    0xff7a: 0x3053, 0xff7b: 0x3055, 0xff7c: 0x3057, 0xff7d: 0x3059, 0xff7e: 0x305b,
                    0xff7f: 0x305d, 0xff80: 0x305f, 0xff81: 0x3061, 0xff82: 0x3064, 0xff83: 0x3066,
                    0xff84: 0x3068, 0xff85: 0x306a, 0xff86: 0x306b, 0xff87: 0x306c, 0xff88: 0x306d,
                    0xff89: 0x306e, 0xff8a: 0x306f, 0xff8b: 0x3072, 0xff8c: 0x3075, 0xff8d: 0x3078,
                    0xff8e: 0x307b, 0xff8f: 0x307e, 0xff90: 0x307f, 0xff91: 0x3080, 0xff92: 0x3081,
                    0xff93: 0x3082, 0xff94: 0x3084, 0xff95: 0x3086, 0xff96: 0x3088, 0xff97: 0x3089,
                    0xff98: 0x308a, 0xff99: 0x308b, 0xff9a: 0x308c, 0xff9b: 0x308d, 0xff9c: 0x308f,
                    0xff9d: 0x3093
                };
                return String.fromCharCode(f[c.charCodeAt(0)]);
            });
        } else if (option.match('k')) {
            // zen-kaku kata-kana to han-kaku kata-kana
            str = str.replace(/[\u3001\u3002\u300c\u300d\u30a1-\u30ab\u30ad\u30af\u30b1\u30b3\u30b5\u30b7\u30b9\u30bb\u30bd\u30bf\u30c1\u30c3\u30c4\u30c6\u30c8\u30ca-\u30cf\u30d2\u30d5\u30d8\u30db\u30de\u30df-\u30ed\u30ef\u30f2\u30f3\u30fb]/g, function (c) {
                var f = {
                    0x3001: 0xff64, 0x3002: 0xff61, 0x300c: 0xff62, 0x300d: 0xff63, 0x30a1: 0xff67,
                    0x30a2: 0xff71, 0x30a3: 0xff68, 0x30a4: 0xff72, 0x30a5: 0xff69, 0x30a6: 0xff73,
                    0x30a7: 0xff6a, 0x30a8: 0xff74, 0x30a9: 0xff6b, 0x30aa: 0xff75, 0x30ab: 0xff76,
                    0x30ad: 0xff77, 0x30af: 0xff78, 0x30b1: 0xff79, 0x30b3: 0xff7a, 0x30b5: 0xff7b,
                    0x30b7: 0xff7c, 0x30b9: 0xff7d, 0x30bb: 0xff7e, 0x30bd: 0xff7f, 0x30bf: 0xff80,
                    0x30c1: 0xff81, 0x30c3: 0xff6f, 0x30c4: 0xff82, 0x30c6: 0xff83, 0x30c8: 0xff84,
                    0x30ca: 0xff85, 0x30cb: 0xff86, 0x30cc: 0xff87, 0x30cd: 0xff88, 0x30ce: 0xff89,
                    0x30cf: 0xff8a, 0x30d2: 0xff8b, 0x30d5: 0xff8c, 0x30d8: 0xff8d, 0x30db: 0xff8e,
                    0x30de: 0xff8f, 0x30df: 0xff90, 0x30e0: 0xff91, 0x30e1: 0xff92, 0x30e2: 0xff93,
                    0x30e3: 0xff6c, 0x30e4: 0xff94, 0x30e5: 0xff6d, 0x30e6: 0xff95, 0x30e7: 0xff6e,
                    0x30e8: 0xff96, 0x30e9: 0xff97, 0x30ea: 0xff98, 0x30eb: 0xff99, 0x30ec: 0xff9a,
                    0x30ed: 0xff9b, 0x30ef: 0xff9c, 0x30f2: 0xff66, 0x30f3: 0xff9d, 0x30fb: 0xff65
                };
                return String.fromCharCode(f[c.charCodeAt(0)]);
            }).replace(/[\u30ac\u30ae\u30b0\u30b2\u30b4\u30b6\u30b8\u30ba\u30bc\u30be\u30c0\u30c2\u30c5\u30c7\u30c9\u30d0\u30d3\u30d6\u30d9\u30dc\u30f4\u30f7\u30fa]/g, function (c) {
                // with daku-ten
                var f = {
                    0x30ac: 0xff76, 0x30ae: 0xff77, 0x30b0: 0xff78, 0x30b2: 0xff79, 0x30b4: 0xff7a,
                    0x30b6: 0xff7b, 0x30b8: 0xff7c, 0x30ba: 0xff7d, 0x30bc: 0xff7e, 0x30be: 0xff7f,
                    0x30c0: 0xff80, 0x30c2: 0xff81, 0x30c5: 0xff82, 0x30c7: 0xff83, 0x30c9: 0xff84,
                    0x30d0: 0xff8a, 0x30d3: 0xff8b, 0x30d6: 0xff8c, 0x30d9: 0xff8d, 0x30dc: 0xff8e,
                    0x30f4: 0xff73, 0x30f7: 0xff9c, 0x30fa: 0xff66
                }
                return String.fromCharCode(f[c.charCodeAt(0)]) + "\uff9e";
            }).replace(/[\u30d1\u30d4\u30d7\u30da\u30dd]/g, function (c) {
                // with han-daku-ten
                var f = {
                    0x30d1: 0xff8a, 0x30d4: 0xff8b, 0x30d7: 0xff8c, 0x30da: 0xff8d, 0x30dd: 0xff8e
                }
                return String.fromCharCode(f[c.charCodeAt(0)]) + "\uff9f";
            })
                ;
        } else if (option.match('h')) {
            // zen-kaku hira-gana to han-kaku kata-kana
            str = str.replace(/[\u3001\u3002\u300c\u300d\u3041-\u304b\u304d\u304f\u3051\u3053\u3055\u3057\u3059\u305b\u305d\u305f\u3061\u3063\u3064\u3066\u3068\u306a-\u306f\u3072\u3075\u3078\u307b\u307e\u307f-\u308d\u308f\u3092\u3093\u30fb]/g, function (c) {
                var f = {
                    0x3001: 0xff64, 0x3002: 0xff61, 0x300c: 0xff62, 0x300d: 0xff63, 0x3041: 0xff67,
                    0x3042: 0xff71, 0x3043: 0xff68, 0x3044: 0xff72, 0x3045: 0xff69, 0x3046: 0xff73,
                    0x3047: 0xff6a, 0x3048: 0xff74, 0x3049: 0xff6b, 0x304a: 0xff75, 0x304b: 0xff76,
                    0x304d: 0xff77, 0x304f: 0xff78, 0x3051: 0xff79, 0x3053: 0xff7a, 0x3055: 0xff7b,
                    0x3057: 0xff7c, 0x3059: 0xff7d, 0x305b: 0xff7e, 0x305d: 0xff7f, 0x305f: 0xff80,
                    0x3061: 0xff81, 0x3063: 0xff6f, 0x3064: 0xff82, 0x3066: 0xff83, 0x3068: 0xff84,
                    0x306a: 0xff85, 0x306b: 0xff86, 0x306c: 0xff87, 0x306d: 0xff88, 0x306e: 0xff89,
                    0x306f: 0xff8a, 0x3072: 0xff8b, 0x3075: 0xff8c, 0x3078: 0xff8d, 0x307b: 0xff8e,
                    0x307e: 0xff8f, 0x307f: 0xff90, 0x3080: 0xff91, 0x3081: 0xff92, 0x3082: 0xff93,
                    0x3083: 0xff6c, 0x3084: 0xff94, 0x3085: 0xff6d, 0x3086: 0xff95, 0x3087: 0xff6e,
                    0x3088: 0xff96, 0x3089: 0xff97, 0x308a: 0xff98, 0x308b: 0xff99, 0x308c: 0xff9a,
                    0x308d: 0xff9b, 0x308f: 0xff9c, 0x3092: 0xff66, 0x3093: 0xff9d, 0x30fb: 0xff65
                };
                return String.fromCharCode(f[c.charCodeAt(0)]);
            }).replace(/[\u304c\u304e\u3050\u3052\u3054\u3056\u3058\u305a\u305c\u305e\u3060\u3062\u3065\u3067\u3069\u3070\u3073\u3076\u3079\u307c\u3094]/g, function (c) {
                // with daku-ten
                var f = {
                    0x304c: 0xff76, 0x304e: 0xff77, 0x3050: 0xff78, 0x3052: 0xff79, 0x3054: 0xff7a,
                    0x3056: 0xff7b, 0x3058: 0xff7c, 0x305a: 0xff7d, 0x305c: 0xff7e, 0x305e: 0xff7f,
                    0x3060: 0xff80, 0x3062: 0xff81, 0x3065: 0xff82, 0x3067: 0xff83, 0x3069: 0xff84,
                    0x3070: 0xff8a, 0x3073: 0xff8b, 0x3076: 0xff8c, 0x3079: 0xff8d, 0x307c: 0xff8e,
                    0x3094: 0xff73
                };
                return String.fromCharCode(f[c.charCodeAt(0)]) + "\uff9e";
            }).replace(/[\u3071\u3074\u3077\u307a\u307d]/g, function (c) {
                // with han-daku-ten
                var f = {
                    0x3071: 0xff8a, 0x3074: 0xff8b, 0x3077: 0xff8c, 0x307a: 0xff8d, 0x307d: 0xff8e
                };
                return String.fromCharCode(f[c.charCodeAt(0)]) + "\uff9f";
            })
                ;
        }
        if (option.match('c')) {
            // zen-kaku kata-kana to zen-kaku hira-gana
            str = str.replace(/[\u30a1-\u30f6]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) - 0x0060)
            });
        } else if (option.match('C')) {
            // zen-kaku hira-gana to zen-kaku kata-kana
            str = str.replace(/[\u3041-\u3086]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) + 0x0060)
            });
        }
        if (option.match('r')) {
            // zen-kaku alphabets to han-kaku
            str = str.replace(/[\uff21-\uff3a\uff41-\uff5a]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) - 0xFee0)
            });
        } else if (option.match('R')) {
            // han-kaku alphabets to zen-kaku
            str = str.replace(/[A-Za-z]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) + 0xFee0)
            });
        }
        if (option.match('n')) {
            // zen-kaku numbers to han-kaku
            str = str.replace(/[\uff10-\uff19]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) - 0xfee0);
            });
        } else if (option.match('N')) {
            // han-kaku numbers to zen-kaku
            str = str.replace(/[\u0030-\u0039]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) + 0xfee0);
            });
        }
        // Characters included in "a", "A" options are
        // "!" - "~" excluding '"', "'", "\", "~"
        if (option.match('a')) {
            // zen-kaku alphabets and numbers to han-kaku
            str = str.replace(/[\uff01\uff03-\uff06\uff08-\uff3b\uff3d-\uff5d]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) - 0xfee0);
            });
        } else if (option.match('A')) {
            // han-kaku alphabets and numbers to zen-kaku
            str = str.replace(/[\u0021\u0023-\u0026\u0028-\u005b\u005d-\u007d]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) + 0xfee0);
            });
        }
        if (option.match('s')) {
            // zen-kaku space to han-kaku
            str = str.replace(/\u3000/g, "\u0020");
        } else if (option.match('S')) {
            // han-kaku space to zen-kaku
            str = str.replace(/\u0020/g, "\u3000");
        }
        return str;
    }

    //Check browser is Safari in desktop
    function isSafari() {
        return (!!navigator.userAgent.match(/.{1,}Version\/.{1,}Safari\/.{1,}/) && navigator.userAgent.indexOf("Mobile") == -1);
    }

    //Check Safari version higher than 11.1
    function isSecurityUpdatedSafariVersion() {
        if (!isSafari()) {
            return false;
        }

        var versionNumber = navigator.userAgent.split("Version/")[1].split(" Safari")[0]; // get version number between Version/ and Safari

        var majorVersion = versionNumber.split(".")[0];
        var minorVersion = versionNumber.split(".")[1];
        if (majorVersion > 11 || (majorVersion == 11 && minorVersion >= 1)) {
            return true;
        }

        return false;
    }

    function LoadSearchCondition() {
        var listConditionElement = [];
        var listHiddenField = [];
        var pairList = {};
        $('.search-condition').find('input,select').each(function () {
            var id = $(this).attr('id');
            if (id != undefined && id.indexOf("Condition") === 0) {
                listConditionElement.push(id);
            }
        });
        $('.frmExport').find('input').each(function () {
            var id = $(this).attr('id');
            if (id != undefined && id.indexOf("hdnOrderBy") != 0 && id.indexOf("hdnOrderType") != 0) {
                listHiddenField.push(id);
            }
        });

        for (var i = 0; i < listConditionElement.length; i++) {
            pairList[listHiddenField[i]] = listConditionElement[i];
        }

        $.each(pairList, function (key, value) {
            var control = $("#" + value);
            if (control.prop('type') == 'checkbox') {
                $("#" + key).val($("#" + value).prop('checked'));
            }
            else {
                $("#" + key).val($("#" + value).val());
            }
        });
    }

    function ClearRestoreData(controllerName) {
        $.ajax({
            type: "GET",
            url: '/' + controllerName + '/ClearRestoreData',
            dataType: 'json',
            async: false,
            cache: false,
            success: function (result) {
            },
            error: function (err) {
            }
        });
    }

    function SetButtonBackSession() {
        $.ajax({
            type: "POST",
            url: '/Common/SetButtonBackSession',
            dataType: 'json',
            data: null,
            success: function () {
            },
            error: function (error) {
            }
        });
    }

    return {
        formatMoney: formatMoney,
        formatMoneyLabel: formatMoneyLabel,
        convertMoneyToInt: convertMoneyToInt,
        convertIntToMoney: convertIntToMoney,
        replaceAllMoney: replaceAllMoney,
        validDate: validDate,
        validateRangeYear: validateRangeYear,
        compareDate: compareDate,
        compareDateRange: compareDateRange,
        validAlphanumeric: validAlphanumeric,
        validPositiveNumeric: validPositiveNumeric,
        showClientError: showClientError,
        validNumeric: validNumeric,
        validAcount: validAcount,
        validEmail: validEmail,
        validFullHalfSize: validFullHalfSize,
        htmlEncode: htmlEncode,
        htmlDecode: htmlDecode,
        roundingDecimal: roundingDecimal,
        roundDecimalPercent: roundDecimalPercent,
        showMessageDialog: showMessageDialog,
        showSubmitConfirmDialog: showSubmitConfirmDialog,
        IsAuthenticateTimeoutRedirect: IsAuthenticateTimeoutRedirect,
        checkInputNumber: checkInputNumber,
        buildColumShortText: buildColumShortText,
        builDetailFormCode: builDetailFormCode,
        IsAuthenticateTimeout: IsAuthenticateTimeout,
        validURL: validURL,
        focusTextbox: focusTextbox,
        validDecimalNumeric: validDecimalNumeric,
        checkInputPhone: checkInputPhone,
        imeControl: imeControl,
        imeControlNew: imeControlNew,
        getDataByAjax: getDataByAjax,
        checkDataExistByAjax: checkDataExistByAjax,
        getImageByAjax: getImageByAjax,
        getMonthCols: getMonthCols,
        bindMonthCols: bindMonthCols,
        bindTotalCols: bindTotalCols,
        bindTagsByCustomer: bindTagsByCustomer,
        validateControlInput: validateControlInput,
        removeValidationError: removeValidationError,
        validPhone: validPhone,
        checkInputIpAddress: checkInputIpAddress,
        validIpAddress: validIpAddress,
        checkPermission: checkPermission,
        nvl: nvl,
        htmlEncodeByReplace: htmlEncodeByReplace,
        Convert_JPCharacters: Convert_JPCharacters,
        isSecurityUpdatedSafariVersion: isSecurityUpdatedSafariVersion,
        LoadSearchCondition: LoadSearchCondition,
        ClearRestoreData: ClearRestoreData,
        SetButtonBackSession: SetButtonBackSession
    };
}());
