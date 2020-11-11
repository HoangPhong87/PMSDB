
$(function () {
    $(document).off('.numeric');
    $(document).on('focus', '.numeric', function () {
        $(".numeric").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
        // Integer
        $(".integer").css({ "ime-mode": "disabled" }).numeric({ decimal: false });
        // A positive number
        $(".positive").css({ "ime-mode": "disabled" }).numeric({ negative: false });
        // Positive integer
        $(".positive-integer").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
    });

    $(document).off('input.hour');
    $(document).on('focus', 'input.hour', function () {
        $("input.hour").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
    });

    $(document).off('input.hour');
    $(document).on('focusout', 'input.hour', function () {
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
        var data = PMS.utility.validPositiveNumeric(value) ? value : '';
        $(this).val(data);
    });
    $(document).off('input.minute');
    $(document).on('focus', 'input.minute', function () {
        $("input.minute").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
    });

    $(document).off('input.minute');
    $(document).on('focusout', 'input.minute', function () {
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
        var data = PMS.utility.validPositiveNumeric(value) ? value : '';
        $(this).val(data);
    });

    $(document).off('.tell-no');
    $(document).on('focusout', '.tell-no', function () {
        var data = "";
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');

        if (value.indexOf('--') !== -1) {
            data = PMS.utility.validPositiveNumeric(value) ? value : '';
        } else {
            if (value.indexOf('-') !== -1) {
                var tellNo = value;
                for (i = 0; i < value.split('-').length - 1; i++) {
                    tellNo = tellNo.replace('-', '');
                }
                data = PMS.utility.validPositiveNumeric(tellNo) ? value : '';
            } else {
                data = PMS.utility.validPositiveNumeric(value) ? value : '';
            }
        }
        $(this).val(data);
    });

    $(document).off('.zip-code');
    $(document).on('focusout keyup', '.zip-code', function () {
        var zip_code = PMS.utility.Convert_JPCharacters(this.value, 'n');

        var data = "";
        if (zip_code.indexOf('--') !== -1) {
            data = PMS.utility.validPositiveNumeric(zip_code) ? zip_code : '';
        } else {
            if (zip_code.indexOf('-') !== -1) {
                if (zip_code.indexOf('-') !== 3) {
                    for (i = 0; i < zip_code.split('-').length - 1; i++) {
                        zip_code = zip_code.replace('-', '');
                    }
                    data = PMS.utility.validPositiveNumeric(zip_code) ? zip_code.slice(0, 3) + '-' + zip_code.slice(3, zip_code.length) : '';
                }
            } else {
                if ($(this).val().length >= 4) {
                    data = PMS.utility.validPositiveNumeric(zip_code) ? zip_code.slice(0, 3) + '-' + zip_code.slice(3, zip_code.length) : '';
                } else {
                    data = PMS.utility.validPositiveNumeric(zip_code) ? zip_code : '';
                }
            }
        }
        if (data.length > 8) {
            data = data.slice(0, 8);
        }
        if (zip_code.indexOf('-') !== 3)
            $(this).val(data);
    });

    // Event focus in money field
    $(document).off('.money');
    $(document).on('focus', '.money', function () {
        $(this).val($(this).val().replace(/,/g, ''));
    });

    // Event focus out money field
    $(document).off('.money, .numeric');
    $(document).on('focusout', '.money, .numeric', function () {
        var content = PMS.utility.Convert_JPCharacters($(this).val(), 'n');
        var value = $(this).hasClass('money') ? PMS.utility.convertMoneyToInt(content) : content;
        var data = PMS.utility.validPositiveNumeric(value.toString()) ? parseInt(value) : 0;
        if ($(this).hasClass('money')) {
            $(this).val(data.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
        } else if ($(this).hasClass('prg-val') || $(this).hasClass('is-string')) {
            if (!PMS.utility.validPositiveNumeric(content))
                $(this).val('');
        } else {
            $(this).val(data);
        }
    });

    // Event focus out numeric for budget field
    $(document).off('.numeric1');
    $(document).on('focusout', '.numeric1', function () {
        var content = PMS.utility.Convert_JPCharacters($(this).val(), 'n');
        var value = $(this).hasClass('money') ? PMS.utility.convertMoneyToInt(content) : content;
        var data = PMS.utility.validPositiveNumeric(value.toString()) ? parseInt(value) : '';
        if ($(this).hasClass('money')) {
            $(this).val(data.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
        } else if ($(this).hasClass('prg-val') || $(this).hasClass('is-string')) {
            if (!PMS.utility.validPositiveNumeric(content))
                $(this).val('');
        } else {
            $(this).val(data);
        }
    });

    $(document).off('.decimal');
    $(document).on('focus', '.decimal', function () {
        $(this).css({ "ime-mode": "disabled" }).numeric({ decimal: '.', negative: false, decimalPlaces: 1 });
    });

    // Event focus out decimal field
    $(document).off('.decimal');
    $(document).on('blur', '.decimal', function () {
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
        $(this).val(value);

        if (!PMS.utility.validDecimalNumeric(value)) {
            $(this).val('0.0');
        }
        else {
            value = parseFloat(value) + '';
            $(this).val(value);
        }

        var valArr = value.split('.');
        if (valArr[0].length > 4 || (typeof (valArr[1]) !== 'undefined' && (valArr[1].length == 0 || valArr[1].length > 1))) {
            $(this).val('0.0');
            if ($(this).hasClass('ma-man-days')) {
                var thisMonth = $(this).attr('alt');
                var maID = $(this).attr('id');
                $('table.tb-ma-sales-center td.td-ma-sales-detail[headers="' + thisMonth + '"] span.plan-cost[id="' + maID + '"]').text(0);
                $('table.tb-ma-sales-center td.td-ma-sales-detail[headers="' + thisMonth + '"] input.hdnPlanCost[id="' + maID + '"]').val(0);
                $('table.tb-ma-sales-right tr[id="' + maID + '"] td label.plan-cost-by-user').text(0);
            }
        }

        if (value === '0')
            $(this).val('0.0');


        if ($('#frmProjectEdit').length > 0) {
            setSummaryLabels();
            resetAllTotalByMonth();
        }
    });
});
