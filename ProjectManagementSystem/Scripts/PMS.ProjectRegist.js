/*!
 * File: PMS.ProjectRegist.js
 * Company: i-Enter Asia
 * Copyright © 2014 i-Enter Asia. All rights reserved.
 * Project: Project Management System
 * Author: HoangPS
 * Created date: 2014/06/10
 */

var Constant = {
    ERR_REQUIRED: 'は必須項目です。',
    ERR_FORMAT: 'の入力形式に誤りがあります。',
    ERR_COMPARE_DATE: '期間終了日は期間開始日より未来の日付を入力して下さい。',
    ERR_COMPARE_ACCEPTANCE_DATE: '検収日は終了日以降の日付を設定してください。',
    ERR_REQUIRED_CHOOSE: 'を選択してください。',
    ERR_REQUIRED_SELECT: 'を指定してください。',
    ERR_REQUIRED_TIME: 'を設定してください。',
    ERR_REQUIRED_FILE_NAME: '添付ファイルの表示名を設定してください。',
    ERR_REQUIRED_PROGRESS: '未入力の進捗があります。',
    ERR_REQUIRED_SUBCONTRACTOR_PIC: '発注先担当者を選択してください。',
    ERR_REQUIRED_OVERHEADCOST_PIC: '諸経費担当者を選択してください。',
    ERR_STRING_LENGTH: '{0}は{1}文字以内で入力してください。',
    ERR_RANGE: '{0}は0～{1}の範囲で入力してください。',
    ERR_COMPARE_TOTAL_ESTIMATE: 'アサイン一覧の合計工数が一致しません。',
    ERR_COMPARE_TOTAL_SALES: '個人合計と支払金額の合計が受注金額と一致しません。',
    ERR_COMPARE_TOTAL_PAYMENT: '支払金額と支払一覧の合計金額が一致しません。',
    ERR_COMPARE_TOTAL_SUPPLIER: '発注先金額と発注先一覧の合計金額が一致しません。',
    ERR_COMPARE_TOTAL_OVERHEADCOST: '諸経費金額と諸経費一覧の合計金額が一致しません。',
    ERR_MAX_MONTH_PROJECT_DURATION: '期間は3年以内で設定してください。',
    ERR_DELETE_MEMBER: '対象のユーザーは稼働実績が入力されているため削除できません。',
    ERR_DELETE_PHASE: '対象のフェーズは既に使用されているため除外できません。',
    ERR_CHANGE_CONTRACT_TYPE: '既に稼動実績が入力されているため変更できません。',
    ERR_CHANGE_DURATION_TIME: '既に稼動実績が入力されているため期間を短縮できません。',
    ERR_CONNECT_TIMEOUT: '接続の有効時間が切れました。再度ログインしなおしてください。',
    ERR_DUPLICATE_PROGRESS_REGIST_DATE: '同じ日付で進捗が登録されています。',
    ERR_PROGRESS_REGIST_DATE_REQUIRED_INPUT: '未入力の進捗登録日があります。',
    ERR_FORMAT_PROGRESS_REGIST_DATE: '進捗登録日の形式に誤りがあります。',
    ERR_CANNOT_ASSIGN_MEMBER: '勤務情報入力済みの本登録されているデータが存在するためアサインできません。',
    ERR_DELETE_PIC: '担当者に設定されているユーザーは削除できません。',
    ERR_ASSIGN_PIC: 'アサイン一覧に担当者を追加してください。',
    CONFIRM_CHANGE_COLUM: '期間が変更されました。<br>アサイン一覧、売上一覧、支払一覧の内容が変更されますがよろしいですか？',
    CONFIRM_CHANGE_TAX_RATE: '消費税率を変更しますか？',
    SUBCONTRACTOR_HAS_BEEN_SELECTED: '既に選択されている発注先です。',
    ORDER_HAS_BEEN_SELECTED: '既に選択されている発注元です。',
    END_USER_HAS_BEEN_SELECTED: '既に選択されているエンドユーザーです。',
    USER_HAS_BEEN_SELECTED: '既に選択されているメンバーです。',
    SUBCATEGORY_HAS_BEEN_SELECTED: '選択済みのサブカテゴリです。他のサブカテゴリを選択してください。',
    PROJECT_NO: 'プロジェクトNo.',
    PROJECT_NAME: 'プロジェクト名',
    CUSTOMER_NAME: '発注元',
    ESTIMATE_MANDAY: '見積工数',
    PROGRESS: '進捗',
    PROGRESS_REGIST_DATE: '登録日',
    PERIOD_START: '期間（開始）',
    PERIOD_END: '期間（終了）',
    ACCEPTANCE_DATE: '検収日',
    PROJECT_STATUS: 'ステータス',
    CHARGE_PERSON: '担当者',
    TARGET_PHASE: '対象フェーズ',
    SUB_CATEGORY: 'サブカテゴリ',
    TOTAL_SALES: '売上金額',
    TOTAL_PAYMENT: '支払金額',
    TOTAL_AMOUNT_SUBCONTRACTOR: '発注先の合計金額',
    TOTAL_AMOUNT_OVERHEADCOST: '諸経費の合計金額',
    TAX_RATE: '消費税率',
    UNIT_PRICE_MEMBER_LIST: 'アサイン一覧の単価',
    ESTIMATE_TIME_MEMBER_LIST: 'アサイン一覧の予定工数',
    INDIVIDUAL_SALES_LIST: '個人売上',
    AMOUNT_PAYMENT_LIST: '発注先一覧の金額',
    AMOUNT_OVERHEADCOST_LIST: '諸経費一覧の金額',
    NONE_SPECIFIED: '指定なし',
    DATE_FORMAT: 'yyyy/mm/dd',
    SHORT_DATE_FORMAT: 'yyyy/mm',
    MAX_MONEY: '999,999,999',
    MAX_PROJECT_NAME: '50',
    MAX_CUSTOMER_NAME: '100',
    MAX_MANDAY: '9999',
    MAX_PERCENT: '100',
    MAX_FILE_TITLE: 100,
    MAX_AREA_TEXT: 2000,
    MAX_MONTH_PROJECT_DURATION: 36,
    URL_REDIRECT_TIMEOUT: '/PMS01001/Login/timeout',
    ESTIMATE_TIME_MAX_LENGTH: 6,
    DECIMAL_AFTER_POINT_MAX_LENGTH: 1,
    NEW_MEMBER_TITLE: 'new',
    DIALOG_INFORMATION: "<i class='fa fa-check-circle'></i>",
    DIALOG_WARNING: "<i class='fa fa-warning'></i>",
    COPY_PRJ_TYPE: {
        NORMAL: '0',
        ALL_INFORMATION: '1'
    }
}

$(document).ready(function () {
    PMS.utility.imeControl($("#PROJECT_INFO_project_name, #PROJECT_INFO_status_note, #PROJECT_INFO_remarks, .progress-remarks"), 'active');
    PMS.utility.imeControl($("#PROJECT_INFO_total_sales, #PROJECT_INFO_tax_rate, .ime-mode, .money, .numeric"), 'disable');

    var projectID = parseInt($('#PROJECT_INFO_project_sys_id').val());
    var startDate = $('#PROJECT_INFO_start_date').val();
    var endDate = $('#PROJECT_INFO_end_date').val();

    $('.content-wrapper').addClass('no-padding-bottom');

    PMS.utility.bindMonthCols(startDate, endDate);
    PMS.utility.bindTotalCols(null, startDate, endDate);

    if (projectID === 0 && $("#COPY_TYPE").val() == Constant.COPY_PRJ_TYPE.NORMAL) {
        $('#updateTime, #deleteFlag, #projectNo, .project-plan').hide();
        $('#PROJECT_INFO_estimate_man_days').val('0.0');
        $('.total-estimate-man-days').text('0.0');

        $('#lblEstimate').text('0.0人日');
        setSalesType();

        if ($("#IS_CREATE_COPY").val() == "True") {
            var newMember = {
                USER_ID: $("#PROJECT_INFO_charge_person_id").val(),
                USER_NAME: PMS.utility.htmlEncode($("#PROJECT_INFO_charge_person").val()),
                BASE_UNIT_COST: $("#PROJECT_INFO_base_unit_cost").val()
            };

            bindNewMemberAssignment([newMember], false);
        }
    } else {
        //#70214
        if ($("#COPY_TYPE").val() == Constant.COPY_PRJ_TYPE.ALL_INFORMATION) {
            projectID = parseInt($('#PRJ_SYS_ID_TO_COPY').val());
        }

        loadDataOnTableList(projectID, $('#PROJECT_INFO_temp_start_date').val(), $('#PROJECT_INFO_temp_end_date').val());
    }

    $('#hdnTempStart, #hdnOldStart').val(startDate);
    $('#hdnTempTo, #hdnOldTo').val(endDate);
    $('#PROJECT_INFO_status_id').attr('oldValue', $('#PROJECT_INFO_status_id option:selected').attr('data-type'));
    $('#lblStatusInfo').html($('#PROJECT_INFO_status_id option:selected').text());

    displayChargeOfSales();
    displaySubcontractor();
    setArrCategory();
    resetArrProgress();
    resetAllTotalPhaseEffort();
    PMS.utility.formatMoney();
    PMS.utility.formatMoneyLabel();
    PMS.utility.focusTextbox();

    setEstimateLabel();
    resetAllTotalByMonth();
    $('.project-summary-small').hide();
    $('.project-summary-small').height($('.project-summary').height());
    $('.project-summary-title-small').height($('.project-summary').height() - 32);
});

// Validate data input
function validateData() {
    var invalidMess = [];
    var projectName = $('#PROJECT_INFO_project_name').val().trim();
    var invalidProjectName = false;

    if (projectName.length === 0) { // required project name
        invalidMess.push(Constant.PROJECT_NAME + Constant.ERR_REQUIRED);
        invalidProjectName = true;
    } else if (projectName.length > 50) { // valid string length
        invalidMess.push(Constant.ERR_STRING_LENGTH.replace('{0}', Constant.PROJECT_NAME).replace('{1}', Constant.MAX_PROJECT_NAME));
        invalidProjectName = true;
    }

    if (invalidProjectName)
        $("label[for='project_name']").addClass("label-validation-error");

    var projectEstimate = $('#PROJECT_INFO_estimate_man_days').val().trim();
    var validProjectEstimate = true;

    if (projectEstimate.length === 0) { // required project estimate
        invalidMess.push(Constant.ESTIMATE_MANDAY + Constant.ERR_REQUIRED);
        validProjectEstimate = false;
    } else { // validate input estimate
        var projectEstimateArr = projectEstimate.split('.');

        if (!PMS.utility.validDecimalNumeric(projectEstimate)
            || (typeof projectEstimateArr[1] != 'undefined'
                && (projectEstimateArr[1].length == 0
                    || projectEstimateArr[1].length > Constant.DECIMAL_AFTER_POINT_MAX_LENGTH))) {
            invalidMess.push(Constant.ESTIMATE_MANDAY + Constant.ERR_FORMAT);
            validProjectEstimate = false;
        } else if (projectEstimate.length > Constant.ESTIMATE_TIME_MAX_LENGTH || projectEstimateArr[0].length > 4) {
            invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.ESTIMATE_MANDAY).replace('{1}', Constant.MAX_MANDAY));
            validProjectEstimate = false;
        }
    }

    if (!validProjectEstimate)
        $("label[for='estimate_man_days']").addClass("label-validation-error");

    var customerName = $('#OUTSOURCER_customer_name').val().trim();
    var invalidCustomerName = false;

    if (customerName.length === 0) { // required customer name
        invalidMess.push(Constant.CUSTOMER_NAME + Constant.ERR_REQUIRED_CHOOSE);
        invalidProjectName = true;
    } else if (customerName.length > 100) { // valid string length
        invalidMess.push(Constant.ERR_STRING_LENGTH.replace('{0}', Constant.CUSTOMER_NAME).replace('{1}', Constant.MAX_CUSTOMER_NAME));
        invalidProjectName = true;
    }

    if (invalidProjectName)
        $("label[for='customer_name']").addClass("label-validation-error");

    var startDate = $('#PROJECT_INFO_start_date').val().trim();
    var endDate = $('#PROJECT_INFO_end_date').val().trim();
    var existInvalidDate = false;

    if (startDate.length === 0) { // required start date
        existInvalidDate = true;
        invalidMess.push(Constant.PERIOD_START + Constant.ERR_REQUIRED);
    } else { // validate start date
        var errStartDate = PMS.utility.validDate(startDate, Constant.DATE_FORMAT, Constant.PERIOD_START);
        if (errStartDate != null) {
            existInvalidDate = true;
            invalidMess.push(errStartDate);
        }
    }

    if (endDate.length === 0) { // required end date
        existInvalidDate = true;
        invalidMess.push(Constant.PERIOD_END + Constant.ERR_REQUIRED);
    } else { // validate end date
        var errEndDate = PMS.utility.validDate(endDate, Constant.DATE_FORMAT, Constant.PERIOD_END);
        if (errEndDate != null) {
            existInvalidDate = true;
            invalidMess.push(errEndDate);
        }
    }

    // compare period
    if (!existInvalidDate &&
        !PMS.utility.compareDate(startDate, endDate, Constant.DATE_FORMAT)) {
        existInvalidDate = true;
        invalidMess.push(Constant.ERR_COMPARE_DATE);
    }

    if (!PMS.utility.validateRangeYear(startDate, endDate, Constant.MAX_MONTH_PROJECT_DURATION)) {
        existInvalidDate = true;
        invalidMess.push(Constant.ERR_MAX_MONTH_PROJECT_DURATION);
    }

    if (existInvalidDate)
        $("label[for='duration_time']").addClass("label-validation-error");

    var acceptanceDate = $('#PROJECT_INFO_acceptance_date').val().trim();

    // validate acceptance date
    if (acceptanceDate.length > 0) {
        var errAcceptanceDate = PMS.utility.validDate(acceptanceDate, Constant.DATE_FORMAT, Constant.ACCEPTANCE_DATE);

        if (errAcceptanceDate != null) {
            invalidMess.push(errAcceptanceDate);
            $("label[for='PROJECT_INFO_acceptance_date']").addClass("label-validation-error");
        }
    }

    var picID = $('#PROJECT_INFO_charge_person_id').val();

    if (isNaN(parseInt(picID)) || parseInt(picID) === 0) { // required charge person
        invalidMess.push('プロジェクト責任者を選択してください。');
        $("label[for='charge_person']").addClass("label-validation-error");
    } else {
        // Validatation of exist PIC on assign list
        if ($('table.tb-ma-left tr[id="' + picID + '"]').length == 0) {
            invalidMess.push(Constant.ERR_ASSIGN_PIC);
            $("label[for='assign_member_list']").addClass("label-validation-error");
        }
    }

    var existTargetPhase = false;

    $('input.target-phase').each(function () {
        if ($(this).prop('checked')) {
            existTargetPhase = true;
            return false;
        }
    });

    if (!existTargetPhase) { // required target phase
        $("label.lblPhaseList").addClass("label-validation-error");
        invalidMess.push(Constant.TARGET_PHASE + Constant.ERR_REQUIRED_CHOOSE);
    }

    var unselectSubCategory = false;

    $('select.ddlSubCategory').each(function () {
        if (typeof ($(this).val()) == 'string' && $(this).val().length == 0) {
            unselectSubCategory = true;
            return false;
        }
    });

    if (unselectSubCategory) { // required select sub category
        $("label[for='category_info']").addClass("label-validation-error");
        invalidMess.push(Constant.SUB_CATEGORY + Constant.ERR_REQUIRED_SELECT);
    }

    var projectTotalSales = $('#PROJECT_INFO_total_sales').val().trim().replace(/,/g, '');
    var validProjectTotalSales = true;

    if (projectTotalSales.length === 0) { // required total sales
        invalidMess.push(Constant.TOTAL_SALES + Constant.ERR_REQUIRED);
        validProjectTotalSales = false;
    } else { // validate input total sales
        if (!PMS.utility.validPositiveNumeric(projectTotalSales)) {
            invalidMess.push(Constant.TOTAL_SALES + Constant.ERR_FORMAT);
            validProjectTotalSales = false;
        } else if (projectTotalSales.length > 9) {
            invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.TOTAL_SALES).replace('{1}', Constant.MAX_MONEY));
            validProjectTotalSales = false;
        }
    }

    if (!validProjectTotalSales)
        $("label[for='total_sales']").addClass("label-validation-error");

    var projectTotalPayment = $('#PROJECT_INFO_total_payment').val().length > 0 ? $('#PROJECT_INFO_total_payment').val().trim().replace(/,/g, '') : 0;
    var validProjectTotalPayment = true;
    var isValidPaymentAmount = true;
    var isValidOvcAmount = true;

    if (projectTotalPayment.length > 0) {
        if (!PMS.utility.validPositiveNumeric(projectTotalPayment)) {
            invalidMess.push(Constant.TOTAL_PAYMENT + Constant.ERR_FORMAT);
            validProjectTotalPayment = false;
        } else if (projectTotalPayment.length > 9) {
            invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.TOTAL_PAYMENT).replace('{1}', Constant.MAX_MONEY));
            validProjectTotalPayment = false;
        }

        var $scAmountList = $('input.sc-payment');

        for (var i = 0; i < $scAmountList.length; i++) {
            var scAmount = $($scAmountList).val().replace(/,/g, '');
            if (scAmount.length > 0 && !PMS.utility.validPositiveNumeric(scAmount)) {
                invalidMess.push(Constant.TOTAL_AMOUNT_SUBCONTRACTOR + Constant.ERR_FORMAT);
                isValidPaymentAmount = false;
                break;
            } else if (scAmount.length > 9) {
                invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.TOTAL_AMOUNT_SUBCONTRACTOR).replace('{1}', Constant.MAX_MONEY));
                isValidPaymentAmount = false;
                break;
            }
        }

        var $ovcAmountList = $('input.ovc-total-amount');

        for (var i = 0; i < $ovcAmountList.length; i++) {
            var ovcAmount = $($ovcAmountList).val().replace(/,/g, '');
            if (ovcAmount.length > 0 && !PMS.utility.validPositiveNumeric(ovcAmount)) {
                invalidMess.push(Constant.TOTAL_AMOUNT_OVERHEADCOST + Constant.ERR_FORMAT);
                isValidOvcAmount = false;
                break;
            } else if (ovcAmount.length > 9) {
                invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.TOTAL_AMOUNT_OVERHEADCOST).replace('{1}', Constant.MAX_MONEY));
                isValidOvcAmount = false;
                break;
            }
        }
    }

    var taxRate = $('#PROJECT_INFO_tax_rate').val();
    var invalidTaxRate = false;

    if (!PMS.utility.validPositiveNumeric(taxRate)) { // validate input tax rate
        invalidTaxRate = true;
        invalidMess.push(Constant.TAX_RATE + Constant.ERR_FORMAT);
    } else if (parseInt(taxRate) > 999) {
        invalidTaxRate = true;
        invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.TAX_RATE).replace('{1}', '999'));
    }

    if (invalidTaxRate)
        $("label[for='tax_rate']").addClass("label-validation-error");

    var $unitPriceList = $('input.unit-cost');
    var invalidUnitCost = false;

    if ($unitPriceList.length > 0) {
        for (var i = 0; i < $unitPriceList.length; i++) { // valid input unit price
            var unitPrice = $($unitPriceList).val().replace(/,/g, '');

            if (unitPrice.length > 0) {
                if (!PMS.utility.validPositiveNumeric(unitPrice)) {
                    invalidUnitCost = true;
                    invalidMess.push(Constant.UNIT_PRICE_MEMBER_LIST + Constant.ERR_FORMAT);
                    break;
                } else if (unitPrice.length > 9) {
                    invalidUnitCost = true;
                    invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.UNIT_PRICE_MEMBER_LIST).replace('{1}', Constant.MAX_MONEY));
                    break;
                }
            }
        }

        if (invalidUnitCost)
            $("label[for='assign_member_list']").addClass("label-validation-error");
    }

    var salesType = parseInt($('#PROJECT_INFO_status_id option:selected').attr('data-type'));
    var exceptionCalculate = parseInt($('#PROJECT_INFO_contract_type_id option:selected').attr('data-exception'));

    // Validate data on member assignment table
    var $estimateList = $('input.ma-man-days');
    if ($estimateList.length > 0) {
        var existInvalid = false;

        for (var i = 0; i < $estimateList.length; i++) { // valid input estimate
            var estimate = $($estimateList[i]).val();

            if (estimate.length > 0) {
                var estimateArr = estimate.split('.');
                if (!PMS.utility.validDecimalNumeric(estimate) || (typeof estimateArr[1] != 'undefined' && (estimateArr[1].length == 0 || estimateArr[1].length > Constant.DECIMAL_AFTER_POINT_MAX_LENGTH))) {
                    invalidMess.push(Constant.ESTIMATE_TIME_MEMBER_LIST + Constant.ERR_FORMAT);
                    existInvalid = true;
                    break;
                } else if (estimate.length > Constant.ESTIMATE_TIME_MAX_LENGTH || estimateArr[0].length > 4) {
                    invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.ESTIMATE_TIME_MEMBER_LIST).replace('{1}', Constant.MAX_MANDAY));
                    existInvalid = true;
                    break;
                }
            }
        }
    }

    // Validate data on individual sales table
    var $maSalesList = $('input.ma-sales');
    var totalMaSales = 0;
    var existInvalidMaSales = false;

    if ($maSalesList.length > 0) {
        for (var i = 0; i < $maSalesList.length; i++) { // valid input member individual sales amount
            var maSalesAmount = $($maSalesList[i]).val().replace(/,/g, '');

            if (maSalesAmount.length > 0) {
                if (!PMS.utility.validPositiveNumeric(maSalesAmount)) {
                    invalidMess.push(Constant.INDIVIDUAL_SALES_LIST + Constant.ERR_FORMAT);
                    existInvalidMaSales = true;
                    break;
                } else if (maSalesAmount.length > 9) {
                    invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.INDIVIDUAL_SALES_LIST).replace('{1}', Constant.MAX_MONEY));
                    existInvalidMaSales = true;
                    break;
                }

                totalMaSales += parseInt(maSalesAmount);
            }
        }
    }

    if (salesType == 0
        && exceptionCalculate == 0
        && !existInvalidMaSales
        && validProjectTotalSales
        && validProjectTotalPayment
        && (totalMaSales !== (parseInt(projectTotalSales) - parseInt(projectTotalPayment)))) { // compare total member individual sales with total sales
        existInvalidMaSales = true;
        invalidMess.push(Constant.ERR_COMPARE_TOTAL_SALES);
    }

    // Validate data on subcontractor table
    var $scList = $('div.subcontractor-content.has-payment');
    var invalidSupplier = false;
    var totalPayment = 0;
    var existSubcontractor = false;

    if ($scList.length > 0) {
        var validRequiredSubcontractorPic = true;
        var isValidComparePaymentAmount = true;
        var invalidPaymentAmount = false;

        for (var i = 0; i < $scList.length; i++) {
            var scID = $($scList[i]).children('.sc-id').val();
            var scPicID = $($scList[i]).children('.sc-pic-id').val();

            if (scID.length > 0) {
                if (scPicID.length == 0) {
                    validRequiredSubcontractorPic = false;
                    invalidMess.push(Constant.ERR_REQUIRED_SUBCONTRACTOR_PIC);
                    break;
                }

                existSubcontractor = true;
                var paymentVal = PMS.utility.convertMoneyToInt($($scList[i]).children('.sc-payment').val());
                var totalPaymentAmount = 0;
                var existInvalid = false;

                $('input.payment-amount[id="' + scID + '"]').each(function () {
                    var paymentAmount = $(this).val().replace(/,/g, '');

                    if (paymentAmount.length > 0) {
                        if (!PMS.utility.validPositiveNumeric(paymentAmount)) {
                            invalidMess.push(Constant.AMOUNT_PAYMENT_LIST + Constant.ERR_FORMAT);
                            existInvalid = true;
                            return false;
                        } else if (paymentAmount.length > 9) {
                            invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.AMOUNT_PAYMENT_LIST).replace('{1}', Constant.MAX_MONEY));
                            existInvalid = true;
                            return false;
                        }
                        totalPaymentAmount += parseInt(paymentAmount);
                    }
                });
                totalPayment += totalPaymentAmount;
                invalidPaymentAmount = existInvalid;

                if (salesType == 0
                    //&& exceptionCalculate == 0
                    && existSubcontractor
                    && validProjectTotalPayment
                    && isValidPaymentAmount
                    && !existInvalid
                    && paymentVal != totalPaymentAmount) {
                    invalidMess.push(Constant.ERR_COMPARE_TOTAL_SUPPLIER);
                    isValidComparePaymentAmount = false;
                    invalidPaymentAmount = true;
                    break;
                }
            }
        }

        if (!isValidComparePaymentAmount || !validRequiredSubcontractorPic || invalidPaymentAmount)
            invalidSupplier = true;
    }

    // Validate data on overhead cost table
    var $ovcList = $('div.overheadcost-content.has-payment');
    var invalidOvc = false;
    var ovcTotal = 0;

    if ($ovcList.length > 0) {
        var existOvc = false;
        var validOvcDetail = true;
        var validRequiredOvcPic = true;
        var isValidCompareOvcAmount = true;
        var invalidOvcAmount = false;

        for (var i = 0; i < $ovcList.length; i++) {
            var ovcId = $($ovcList[i]).children('.ovc-id').val();
            var ovcPicID = $($ovcList[i]).children('.ovc-pic-id').val();

            if (ovcId.length > 0) {
                if ($($ovcList[i]).children('.ovc-detail').val().length > 100) {
                    validOvcDetail = false;
                    invalidMess.push(Constant.ERR_STRING_LENGTH.replace('{0}', '諸経費内容').replace('{1}', '100'));
                    break;
                }

                if (ovcPicID.length == 0) {
                    validRequiredOvcPic = false;
                    invalidMess.push(Constant.ERR_REQUIRED_OVERHEADCOST_PIC);
                    break;
                }

                existOvc = true;
                var ovcTotalAmount = PMS.utility.convertMoneyToInt($($ovcList[i]).children('.ovc-total-amount').val());
                var totalAmount = 0;
                var existInvalid = false;

                $('input.ovc-amount[id="' + ovcId + '"]').each(function () {
                    var ovcAmount = $(this).val().replace(/,/g, '');

                    if (ovcAmount.length > 0) {
                        if (!PMS.utility.validPositiveNumeric(ovcAmount)) {
                            invalidMess.push(Constant.AMOUNT_OVERHEADCOST_LIST + Constant.ERR_FORMAT);
                            existInvalid = true;
                            return false;
                        }

                        if (ovcAmount.length > 9) {
                            invalidMess.push(Constant.ERR_RANGE.replace('{0}', Constant.AMOUNT_OVERHEADCOST_LIST).replace('{1}', Constant.MAX_MONEY));
                            existInvalid = true;
                            return false;
                        }
                        totalAmount += parseInt(ovcAmount);
                    }
                });

                ovcTotal += totalAmount;
                invalidOvcAmount = existInvalid;

                if (existInvalid)
                    break;

                if (salesType == 0
                    //&& exceptionCalculate == 0
                    && existOvc
                    && validProjectTotalPayment
                    && isValidOvcAmount
                    && !existInvalid
                    && ovcTotalAmount !== totalAmount) {
                    invalidMess.push(Constant.ERR_COMPARE_TOTAL_OVERHEADCOST);
                    isValidCompareOvcAmount = false;
                    invalidOvcAmount = true;
                    break;
                }
            }
        }

        if (!isValidCompareOvcAmount || !validRequiredOvcPic || invalidOvcAmount || !validOvcDetail)
            invalidOvc = true;
    }

    if (salesType == 0
        && exceptionCalculate == 0
        && existSubcontractor
        && existOvc
        && !invalidSupplier
        && !invalidOvc
        && (totalPayment + ovcTotal) !== parseInt(projectTotalPayment)) {
        invalidMess.push(Constant.ERR_COMPARE_TOTAL_PAYMENT);
    }

    if ($('#PROJECT_INFO_status_note').val().length > Constant.MAX_AREA_TEXT) { // valid string length
        invalidMess.push(Constant.ERR_STRING_LENGTH.replace('{0}', '状況').replace('{1}', Constant.MAX_AREA_TEXT));
        $("label[for='PROJECT_INFO_status_note']").addClass("label-validation-error");
    }

    if ($('#PROJECT_INFO_remarks').val().length > Constant.MAX_AREA_TEXT) { // valid string length
        invalidMess.push(Constant.ERR_STRING_LENGTH.replace('{0}', '備考').replace('{1}', Constant.MAX_AREA_TEXT));
        $("label[for='PROJECT_INFO_remarks']").addClass("label-validation-error");
    }

    return invalidMess;
}

// Display control input charge of sales
function displayChargeOfSales() {
    var salesPicFlg = $('#PROJECT_INFO_contract_type_id').children('option:selected').attr('data-type') == '0' ? false : true;
    var $salesPicContent = $('.sales-pic-content');

    if (!salesPicFlg)
        $salesPicContent.hide().children('input').val('').attr('title', '');

    if (salesPicFlg)
        $salesPicContent.show();
}

// Reset index of element in subcontractor array
function resetArrSubcontractor() {
    $scContentArr = $('div.subcontractor-content');

    for (var i = 0; i < $scContentArr.length; i++) {
        var scSpan = $scContentArr[i];
        var scId = 'SUBCONTRACTOR_LIST[' + i + '].customer_id';
        var scPicId = 'SUBCONTRACTOR_LIST[' + i + '].charge_person_id';
        var scPayment = 'SUBCONTRACTOR_LIST[' + i + '].total_amount';

        $(scSpan).children('.sc-id').attr('name', scId);
        $(scSpan).children('.sc-pic-id').attr('name', scPicId);
        $(scSpan).children('.sc-payment').attr('name', scPayment);
        $(scSpan).children('.sc-name').attr('name', '');
        $(scSpan).children('.sc-pic-name').attr('name', '');
    }
}

// Reset index of element in overhead cost array
function resetArrOverheadCost() {
    $ovcContentArr = $('div.overheadcost-content');

    for (var i = 0; i < $ovcContentArr.length; i++) {
        var ovcSpan = $ovcContentArr[i];
        var ovcId = 'OVERHEAD_COST_LIST[' + i + '].detail_no';
        var ovcTypeId = 'OVERHEAD_COST_LIST[' + i + '].overhead_cost_id';
        var ovcDetail = 'OVERHEAD_COST_LIST[' + i + '].overhead_cost_detail';
        var ovcPicId = 'OVERHEAD_COST_LIST[' + i + '].charge_person_id';
        var ovcTotalAmount = 'OVERHEAD_COST_LIST[' + i + '].total_amount';

        $(ovcSpan).children('.ovc-id').attr('name', ovcId);
        $(ovcSpan).children('.ovc-type-id').attr('name', ovcTypeId);
        $(ovcSpan).children('.ovc-detail').attr('name', ovcDetail);
        $(ovcSpan).children('.ovc-pic-id').attr('name', ovcPicId);
        $(ovcSpan).children('.ovc-total-amount').attr('name', ovcTotalAmount);

        if ($(ovcSpan).hasClass('old-data')) {
            var ovcChange = 'OVERHEAD_COST_LIST[' + i + '].is_change';
            var ovcDelete = 'OVERHEAD_COST_LIST[' + i + '].is_delete';

            $(ovcSpan).children('.ovc-delete').attr('name', ovcDelete);
            $(ovcSpan).children('.ovc-change').attr('name', ovcChange);
        }
    }
}

// Disabled/not disabled or hide/show element
function displaySubcontractor() {
    $('div.has-payment').each(function () {
        var classId = $(this).hasClass('subcontractor-content') ? '.sc-id' : '.ovc-id';
        var value = $(this).children(classId).val();

        if (value.length > 0)
            $(this).children('.sc-payment, .ovc-detail, .ovc-total-amount').removeAttr('readonly');
    });
}

// Caculate total payment
function calTotalPayment() {
    var total = 0;

    $('div.has-payment').each(function () {
        var classAmount = $(this).hasClass('subcontractor-content') ? '.sc-payment' : '.ovc-total-amount';
        var value = $(this).children(classAmount).val();

        if (value.length > 0) {
            var amount = PMS.utility.convertMoneyToInt(value);
            total += amount;
        }
    });

    $('#total_payment').text(PMS.utility.convertIntToMoney(total));
    $('#PROJECT_INFO_total_payment').val(total);
}

// Bind new member assignment to page
function bindNewMemberAssignment(arrNewUser, addType) {
    var colums = PMS.utility.getMonthCols($('#PROJECT_INFO_start_date').val(), $('#PROJECT_INFO_end_date').val());

    var htmlMaLeft = '';
    var htmlMaSalesLeft = '';
    var htmlMaCenter = '';
    var htmlMaSalesCenter = '';
    var htmlMaRight = '';
    var htmlMaSalesRight = '';
    var htmlMaContent = '';

    $('.pic-content').after(htmlMaContent);
    for (var i = 0; i < arrNewUser.length; i++) {
        var maID = arrNewUser[i].USER_ID;
        if (arrNewUser[i].BASE_UNIT_COST == '-') {
            arrNewUser[i].BASE_UNIT_COST = '';
        }

        htmlMaLeft += '<tr id="' + maID + '" alt="' + Constant.NEW_MEMBER_TITLE + '" class="new-data">'
            + ' <td class="td-ma"><div class="ma-display-name text-overflow" title="' + arrNewUser[i].USER_NAME + '">' + arrNewUser[i].USER_NAME + '</div>'
            //+ ' <td class="td-ma right">'
            + ' <input class="ma-id" type="hidden" value="' + maID + '">'
            + ' <input alt="' + maID + '" class="ma-total-manday" type="hidden" value="0">'
            + ' <input alt="' + maID + '" class="ma-total-cost" type="hidden" value="0">'
            //+ ' <input class="money unit-cost right numeric valid w80" type="text" maxlength="9" value="' + arrNewUser[i].BASE_UNIT_COST + '" alt="' + maID + '">'
            //+ ' <label> 円</label>
            + '</td></tr>';
        htmlMaSalesLeft += '<tr id="' + maID + '">'
            + ' <td> <div class="ma-sales-display-name text-overflow" title="' + arrNewUser[i].USER_NAME + '">' + arrNewUser[i].USER_NAME + '</div></td>'
            + '</tr>';
        htmlMaCenter += '<tr id="' + maID + '">';
        htmlMaSalesCenter += '<tr id="' + maID + '">';

        var listUnitCost = arrNewUser[i]["UNIT_COST"];

        for (var j = 0; j < colums.length; j++) {

            var unitCostInMonth = getUnitCostOfMonth(listUnitCost, colums[j])

            htmlMaCenter += ' <td class="td-ma-detail right" headers="' + colums[j] + '">'
                + ' <input class="ma-id" type="hidden" value="' + maID + '">'
                + ' <input class="ma-unit-cost-in-month" type="hidden" id="' + maID + '" alt="' + colums[j] + '" value ="' + unitCostInMonth + '">'
                + ' <input class="ma-target-time" type="hidden" value="' + colums[j] + '">'
                + ' <input id="' + maID + '" class="ma-man-days right decimal valid w80" type="text" alt="' + colums[j] + '" maxlength="6">'
                + ' <label> 人日</label></td>';
            htmlMaSalesCenter += ' <td class="td-ma-sales-detail" headers="' + colums[j] + '">'
                + ' <div class = "div-sales"><div><input id="' + maID + '" class="ma-sales money right numeric valid w80" type="text" alt="' + colums[j] + '" maxlength="9">'
                + ' 円 /</div><div><input type ="hidden" id="' + maID + '" class="hdnPlanCost" alt="' + colums[j] + '" value="0"><span id ="' + maID + '" class="font-normal plan-cost" alt="' + colums[j] + '">0</span>円</div></div></td>';
        }

        htmlMaCenter += '</tr>';
        htmlMaSalesCenter += '</tr>';
        htmlMaRight += '<tr id="' + maID + '"> <td class="right"> <label class="font-normal" id="' + maID + '">0</label> <label> 人日</label></td>'
            + ' </tr>';
        htmlMaSalesRight += '<tr id="' + maID + '">'
            + ' <td class="right"><div class="div-sales"><div><label class="font-normal lbl-money ma-sales-by-user" id="' + maID + '">0</label> <label>円 /&nbsp;</label></div><div><label id="' + maID + '" class="font-normal plan-cost-by-user">0</label><label>&nbsp;円</label></div></div></td>'
            + '</tr>';

        if (addType) {
            htmlMaContent += '<div class="form-group no-bottom col-sm-12 member-content">'
                + ' <label class="col-sm-2 bold">メンバー</label>'
                + ' <div class="col-sm-9 col-input mar-left" data-id="' + maID + '">'
                + ' <input class="value-selected" placeholder="所属" readonly="readonly" title="' + arrNewUser[i].GROUP_NAME + '" type="text" value="' + arrNewUser[i].GROUP_NAME + '">'
                + ' <input class="value-selected" placeholder="名前" readonly="readonly" title="' + arrNewUser[i].USER_NAME + '" type="text" value="' + arrNewUser[i].USER_NAME + '">'
                + ' <button type="button" class="btn light btnChangeMember"><i class="btn-icon btn-search-dialog"></i></button>'
                + ' <label class="lbl-action btnDeleteMa">削除</label>'
                + ' </div>'
                + ' </div>';
        }
    }

    if (addType) {
        if ($('.member-content').length > 0) {
            $('.member-content').last().after(htmlMaContent);
        } else if ($('.sales-pic-content').length > 0) {
            $('.sales-pic-content').after(htmlMaContent);
        } else {
            $('.pic-content').after(htmlMaContent);
        }
    }

    $('table.tb-ma-left tr.tr-total').before(htmlMaLeft);
    $('table.tb-ma-sales-left tr.tr-total').before(htmlMaSalesLeft);
    $('table.tb-ma-center tr.tr-total').before(htmlMaCenter);
    $('table.tb-ma-sales-center tr.tr-total').before(htmlMaSalesCenter);
    $('table.tb-ma-right tr.tr-total').before(htmlMaRight);
    $('table.tb-ma-sales-right tr.tr-total').before(htmlMaSalesRight);

    resetMemberDetail();
    setSummaryLabels();
}

// Set charge person
function setChargePerson(res) {
    if (typeof (res) === 'undefined' || res === null) {
        /// Debug only
        console.log("Could not get outsourcer.");
    } else {
        var memberID = $('#PROJECT_INFO_charge_person_id').val();
        var groupName = $('.pic-content .value-selected').first().val();
        var memberName = $('.pic-content .value-selected').last().val();

        if ($('table.tb-ma-left tr[id="' + res[0].USER_ID + '"]').length == 0) {
            bindNewMemberAssignment([res[0]], false);

            if (memberID != '0') {
                var htmlMaContent = '<div class="form-group no-bottom col-sm-12 member-content">'
                    + ' <label class="col-sm-2 bold">メンバー</label>'
                    + ' <div class="col-sm-9 col-input mar-left" data-id="' + memberID + '">'
                    + ' <input class="value-selected" placeholder="所属" readonly="readonly" title="' + PMS.utility.htmlEncodeByReplace(PMS.utility.htmlEncode(groupName)) + '" type="text" value="' + PMS.utility.htmlEncodeByReplace(PMS.utility.htmlEncode(groupName)) + '">'
                    + ' <input class="value-selected" placeholder="名前" readonly="readonly" title="' + PMS.utility.htmlEncodeByReplace(PMS.utility.htmlEncode(memberName)) + '" type="text" value="' + PMS.utility.htmlEncodeByReplace(PMS.utility.htmlEncode(memberName)) + '">'
                    + ' <button type="button" class="btn light btnChangeMember"><i class="btn-icon btn-search-dialog"></i></button>'
                    + ' <label class="lbl-action btnDeleteMa">削除</label>'
                    + ' </div>'
                    + ' </div>';

                if ($('.member-content').length > 0) {
                    $('.member-content').last().after(htmlMaContent);
                } else if ($('.sales-pic-content').length > 0) {
                    $('.sales-pic-content').after(htmlMaContent);
                } else {
                    $('.pic-content').after(htmlMaContent);
                }
            }
        }
        else {
            if (memberID != '0') {
                var $elementControl = $('div[data-id="' + res[0].USER_ID + '"]');
                $elementControl.children('.value-selected').first().val(groupName).attr('title', groupName);
                $elementControl.children('.value-selected').last().val(memberName).attr('title', memberName);
                $elementControl.attr('data-id', memberID);
            }
            else {
                $('div[data-id="' + res[0].USER_ID + '"]').parent().remove();
            }
        }

        $('#PROJECT_INFO_group_id').val(res[0].GROUP_ID);
        $('#PROJECT_INFO_group_name').val(PMS.utility.htmlDecode(res[0].GROUP_NAME)).attr('title', PMS.utility.htmlDecode(res[0].GROUP_NAME));
        $('#PROJECT_INFO_charge_person_id').val(res[0].USER_ID);
        $('#PROJECT_INFO_charge_person').val(PMS.utility.htmlDecode(res[0].USER_NAME)).attr('title', PMS.utility.htmlDecode(res[0].USER_NAME));
        if (res[0].BASE_UNIT_COST != "-") {
            $('#PROJECT_INFO_base_unit_cost').val(res[0].BASE_UNIT_COST);
        }
        else {
            $('#PROJECT_INFO_base_unit_cost').val('0');
        }
        $('#hdnUserChangeValue').val(true);
    }
}

// Set charge person
function setSalesPic(res) {
    if (typeof (res) === 'undefined' || res === null) {
        /// Debug only
        console.log("Could not get outsourcer.");
    } else {
        $('#PROJECT_INFO_group_sales_pic_id').val(res[0].GROUP_ID);
        $('#PROJECT_INFO_group_sales_pic').val(PMS.utility.htmlDecode(res[0].GROUP_NAME)).attr('title', PMS.utility.htmlDecode(res[0].GROUP_NAME));
        $('#PROJECT_INFO_charge_of_sales_id').val(res[0].USER_ID);
        $('#PROJECT_INFO_charge_of_sales').val(PMS.utility.htmlDecode(res[0].USER_NAME)).attr('title', PMS.utility.htmlDecode(res[0].USER_NAME));
        $('#hdnUserChangeValue').val(true);
    }
}

// Set Outsourcer
function setOutsourcer(res) {
    if (typeof (res) === 'undefined' || res === null) {
        /// Debug only
        console.log("Could not get outsourcer.");
    } else {
        if ($('.out-sourcer input.os-id').val() == res[0].CUSTOMER_ID) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ORDER_HAS_BEEN_SELECTED);
            return;
        }

        /// Display selected outsourcer
        var $osSpan = $('.out-sourcer');
        $osSpan.children('.os-id').val(res[0].CUSTOMER_ID);
        $osSpan.children('.os-name').val(PMS.utility.htmlDecode(res[0].CUSTOMER_NAME)).attr('title', PMS.utility.htmlDecode(res[0].CUSTOMER_NAME));

        PMS.utility.bindTagsByCustomer(res[0].CUSTOMER_ID, '.ddlTagLink');
        $('#hdnUserChangeValue').val(true);
    }
}

// Set End User
function setEndUser(res) {
    if (typeof (res) === 'undefined' || res === null) {
        /// Debug only
        console.log("Could not get end user.");
    } else {
        if ($('.end-user input.eu-id').val() == res[0].CUSTOMER_ID) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.END_USER_HAS_BEEN_SELECTED);
            return;
        }

        /// Display selected end user
        var $osSpan = $('.end-user');
        $osSpan.children('.eu-id').val(res[0].CUSTOMER_ID);
        $osSpan.children('.eu-name').val(PMS.utility.htmlDecode(res[0].CUSTOMER_NAME)).attr('title', PMS.utility.htmlDecode(res[0].CUSTOMER_NAME));

        //PMS.utility.bindTagsByCustomer(res[0].CUSTOMER_ID, '.ddlTagLink');
        $('#hdnUserChangeValue').val(true);
    }
}


// Bind html subcontractor
function bindHTMLSubcontractorList(data, colums, html) {
    // bind html subcontractor list
    html.htmlLeft += '<tr id="' + data.CUSTOMER_ID + '" class="new-data">'
        + ' <td class="td-payment-name"> <div class="text-overflow" title="' + data.CUSTOMER_NAME + '">' + data.CUSTOMER_NAME + '</div></td>'
        + '</tr>';
    html.htmlCenter += '<tr id="' + data.CUSTOMER_ID + '">';

    for (var j = 0; j < colums.length; j++) {
        html.htmlCenter += ' <td class="td-payment-detail right" headers="' + colums[j] + '">'
            + ' <input class="payment-customer-id" type="hidden" value="' + data.CUSTOMER_ID + '">'
            + ' <input class="payment-target-time" type="hidden" value="' + colums[j] + '">'
            + ' <input class="money payment-amount right numeric valid w80" maxlength="9" type="text" id="' + data.CUSTOMER_ID + '"  alt="' + colums[j] + '">'
            + ' <label> 円</label></td>';
    }
    html.htmlCenter += '</tr>';
    html.htmlRight += '<tr id="' + data.CUSTOMER_ID + '"> <td class="right"> <label class="font-normal lbl-money" id="' + data.CUSTOMER_ID + '">0</label> <label> 円</label></td></tr>';

    return html;
}

// Set Subcontractor
function setSubcontractor(res) {
    if (typeof (res) === 'undefined' || res === null) {
        /// Debug only
        console.log("Could not get outsourcer.");
    } else {
        var existCustomer = '';
        var arrNewCustomer = new Array();

        for (var i = 0; i < res.length; i++) {
            var isNewCustomer = true;
            $('div.subcontractor-content .sc-id').each(function () {
                if ($(this).val() == res[i].CUSTOMER_ID) {
                    isNewCustomer = false;
                    existCustomer += '<br>' + res[i].CUSTOMER_NAME;
                }
            });

            if (isNewCustomer)
                arrNewCustomer.push(res[i]);
        }

        if (existCustomer !== '')
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.SUBCONTRACTOR_HAS_BEEN_SELECTED + existCustomer);

        if (arrNewCustomer.length > 0) {
            var htmlSubcontractor = $('div.subcontractor-content').first().prop('outerHTML'); // temp html
            var colums = PMS.utility.getMonthCols($('#PROJECT_INFO_start_date').val(), $('#PROJECT_INFO_end_date').val());
            var html = {
                htmlLeft: '',
                htmlCenter: '',
                htmlRight: ''
            };

            for (var i = 0; i < arrNewCustomer.length; i++) {
                $('div.subcontractor-content').last().after(htmlSubcontractor); // bind html

                // reset value for new subcontractor
                var $newSubcontractor = $('div.subcontractor-content').last().addClass('has-payment');
                $newSubcontractor.children('input').val('').attr('title', '').attr('value', '');
                $newSubcontractor.children('.sc-id').val(arrNewCustomer[i].CUSTOMER_ID);
                $newSubcontractor.children('.sc-name').attr('title', PMS.utility.htmlDecode(arrNewCustomer[i].CUSTOMER_NAME)).val(PMS.utility.htmlDecode(arrNewCustomer[i].CUSTOMER_NAME));
                $newSubcontractor.children('.sc-payment').val('');
                $newSubcontractor.children('.btnSearchSubcontractorPic').removeAttr('disabled');
                $newSubcontractor.children('button').removeClass('clicked');

                html = bindHTMLSubcontractorList(arrNewCustomer[i], colums, html);
            }

            // append html subcontractor list
            $('table.tb-sc-left tr.tr-total').before(html.htmlLeft);
            $('table.tb-sc-center tr.tr-total').before(html.htmlCenter);
            $('table.tb-sc-right tr.tr-total').before(html.htmlRight);

            // remove content no value
            $('div.subcontractor-content:not(".has-payment")').remove();

            $('#hdnUserChangeValue').val(true);
        }

        PMS.utility.imeControl($(".payment-amount"), 'disable');
        displaySubcontractor();
        resetArrSubcontractor();
        resetArrPaymentDetail();
        resetAllTotalByMonth();
    }
}

// Change Subcontractor
function changeSubcontractor(res) {
    if (typeof (res) === 'undefined' || res === null) {
        /// Debug only
        console.log("Could not get outsourcer.");
    } else {
        var $parent = $('.btnChangeSubcontractor.clicked').parent();
        var oldID = $parent.children('.sc-id').val();

        if (oldID == res[0].CUSTOMER_ID)
            return;

        if ($('.sc-id[value="' + res[0].CUSTOMER_ID + '"]').length > 0) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.SUBCONTRACTOR_HAS_BEEN_SELECTED + res[0].CUSTOMER_NAME);
            return;
        }

        $parent.addClass('has-payment');
        $parent.children('.sc-id').val(res[0].CUSTOMER_ID)
        $parent.children('.sc-name').val(PMS.utility.htmlDecode(res[0].CUSTOMER_NAME)).attr('title', PMS.utility.htmlDecode(res[0].CUSTOMER_NAME));
        $parent.children('.btnSearchSubcontractorPic').removeAttr('disabled');
        $parent.children('.sc-payment').removeAttr('readonly');

        var $tbScLeft = $('.tb-sc-left tr[id="' + oldID + '"]');

        if ($tbScLeft.length > 0) {
            $tbScLeft.find('.text-overflow').text(PMS.utility.htmlDecode(res[0].CUSTOMER_NAME));
            $tbScLeft.attr('id', res[0].CUSTOMER_ID);

            var $tbScCenter = $('.tb-sc-center tr[id="' + oldID + '"]');
            $tbScCenter.find('.payment-customer-id').val(res[0].CUSTOMER_ID);
            $tbScCenter.find('.payment-amount').attr('id', res[0].CUSTOMER_ID);
            $tbScCenter.attr('id', res[0].CUSTOMER_ID);

            $('.tb-sc-right tr[id="' + oldID + '"]').attr('id', res[0].CUSTOMER_ID);
        } else {
            var colums = PMS.utility.getMonthCols($('#PROJECT_INFO_start_date').val(), $('#PROJECT_INFO_end_date').val());
            var html = {
                htmlLeft: '',
                htmlCenter: '',
                htmlRight: ''
            };
            html = bindHTMLSubcontractorList(res[0], colums, html);

            // append html subcontractor list
            $('table.tb-sc-left tr.tr-total').before(html.htmlLeft);
            $('table.tb-sc-center tr.tr-total').before(html.htmlCenter);
            $('table.tb-sc-right tr.tr-total').before(html.htmlRight);
        }

        $('#hdnUserChangeValue, #IS_CHANGE_HISTORY').val(true);

        PMS.utility.imeControl($(".payment-amount"), 'disable');
        resetArrSubcontractor();
        resetArrPaymentDetail();
    }
}

// Set Subcontractor pic
function setSubcontractorPic(res) {
    if (typeof (res) === 'undefined' || res === null) {
        /// Debug only
        console.log("Could not get outsourcer.");
    } else {
        var $scContent = $('div.subcontractor-content .btnSearchSubcontractorPic.clicked').parent();

        $scContent.children('.sc-pic-id').val(res[0].USER_ID);
        $scContent.children('.sc-pic-name').val(PMS.utility.htmlDecode(res[0].USER_NAME)).attr('title', PMS.utility.htmlDecode(res[0].USER_NAME));
        $scContent.children('.btnSearchSubcontractorPic').removeClass('clicked');

        //$('table.tb-sc-left tr[id="' + $scContent.children('.sc-id').val() + '"]')
        //    .find('td.td-payment-pic-name div.text-overflow').attr('title', PMS.utility.htmlDecode(res[0].USER_NAME))
        //    .text(PMS.utility.htmlDecode(res[0].USER_NAME));
        $('#hdnUserChangeValue, #IS_CHANGE_HISTORY').val(true);
    }
}

// Set overhead cost pic
function setOverheadCostPic(res) {
    if (typeof (res) === 'undefined' || res === null) {
        /// Debug only
        console.log("Could not get pic.");
    } else {
        var $ovcContent = $('div.overheadcost-content .clicked').parent();

        $ovcContent.children('.ovc-change').val(true);
        $ovcContent.children('.ovc-pic-id').val(res[0].USER_ID);
        $ovcContent.children('.ovc-pic-name').val(PMS.utility.htmlDecode(res[0].USER_NAME)).attr('title', PMS.utility.htmlDecode(res[0].USER_NAME));
        $ovcContent.children('.button').removeClass('clicked');

        $('#hdnUserChangeValue').val(true);
    }
}

// Set Member
function addAssignMember(res) {
    if (typeof (res) === 'undefined' || res === null) {
        /// Debug only
        console.log("Could not get outsourcer.");
    } else {
        var existUser = '';
        var arrNewUser = new Array();

        for (var i = 0; i < res.length; i++) {
            var isNewUser = true;

            $('table.tb-ma-left tr:not(.tb-header, .tr-total)').each(function () {
                if ($(this).attr('id') == res[i].USER_ID) {
                    isNewUser = false;
                    existUser += '<br>' + res[i].USER_NAME;
                }
            });

            if (isNewUser)
                arrNewUser.push(res[i]);
        }

        if (existUser !== '')
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.USER_HAS_BEEN_SELECTED + existUser);

        if (arrNewUser.length > 0) {
            bindNewMemberAssignment(arrNewUser, true);
            $('#hdnUserChangeValue').val(true);
        }

        PMS.utility.imeControl($(".unit-cost, .ma-man-days, .ma-sales"), 'disable');
    }
}

// Change Member
function changeMember(res) {
    if (typeof (res) === 'undefined' || res === null) {
        /// Debug only
        console.log("Could not get outsourcer.");
    } else {
        var $parent = $('.btnChangeMember.clicked').parent();
        var oldID = $parent.attr('data-id');

        if (oldID == res[0].USER_ID)
            return;

        if ($('div.mar-left[data-id="' + res[0].USER_ID + '"]').length > 0 || res[0].USER_ID == $('#PROJECT_INFO_charge_person_id').val()) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.USER_HAS_BEEN_SELECTED + res[0].USER_NAME);
            return;
        }

        $parent.attr('data-id', res[0].USER_ID);
        $parent.children('input').first().val(PMS.utility.htmlDecode(res[0].GROUP_NAME)).attr('title', PMS.utility.htmlDecode(res[0].GROUP_NAME));
        $parent.children('input').last().val(PMS.utility.htmlDecode(res[0].USER_NAME)).attr('title', PMS.utility.htmlDecode(res[0].USER_NAME));

        var $tbMaLeft = $('.tb-ma-left tr[id="' + oldID + '"]');
        $tbMaLeft.find('.ma-display-name').text(PMS.utility.htmlDecode(res[0].USER_NAME));
        $tbMaLeft.find('.ma-id').val(res[0].USER_ID);
        $tbMaLeft.find(".ma-total-manday[alt='" + oldID + "']").attr('alt', res[0].USER_ID);
        $tbMaLeft.find(".ma-total-cost[alt='" + oldID + "']").attr('alt', res[0].USER_ID);


        $tbMaLeft.attr('id', res[0].USER_ID);

        var $tbMaCenter = $('.tb-ma-center tr[id="' + oldID + '"]');
        $tbMaCenter.find('.ma-id').val(res[0].USER_ID);
        $tbMaCenter.attr('id', res[0].USER_ID);
        $tbMaCenter.find('.ma-unit-cost-in-month[id="' + oldID + '"]').attr('id', res[0].USER_ID);
        $tbMaCenter.find('.ma-man-days[id="' + oldID + '"]').attr('id', res[0].USER_ID);

        $('.tb-ma-right tr[id="' + oldID + '"]').attr('id', res[0].USER_ID);
        $('.tb-ma-right tr[id="' + res[0].USER_ID + '"] label.font-normal').attr('id', res[0].USER_ID);

        var $tbSalesLeft = $('.tb-ma-sales-left tr[id="' + oldID + '"]');
        $tbSalesLeft.find('.ma-sales-display-name').text(PMS.utility.htmlDecode(res[0].USER_NAME));
        $tbSalesLeft.attr('id', res[0].USER_ID);

        $('.tb-ma-sales-center tr[id="' + oldID + '"]').attr('id', res[0].USER_ID);
        $('.tb-ma-sales-center tr[id="' + res[0].USER_ID + '"] input.ma-sales').attr('id', res[0].USER_ID);
        $('.tb-ma-sales-center tr[id="' + res[0].USER_ID + '"] input.hdnPlanCost').attr('id', res[0].USER_ID);
        $('.tb-ma-sales-center tr[id="' + res[0].USER_ID + '"] span.plan-cost').attr('id', res[0].USER_ID);

        $('.tb-ma-sales-right tr[id="' + oldID + '"]').attr('id', res[0].USER_ID);
        $('.tb-ma-sales-right tr[id="' + res[0].USER_ID + '"] .ma-sales-by-user').attr('id', res[0].USER_ID);
        $('.tb-ma-sales-right tr[id="' + res[0].USER_ID + '"] .plan-cost-by-user').attr('id', res[0].USER_ID);

        $('.tb-ma-center tr[id="' + res[0].USER_ID + '"] .ma-unit-cost-in-month[id="' + res[0].USER_ID + '"]').each(function () {
            var unit_cost = getUnitCostOfMonth(res[0].UNIT_COST, $(this).attr('alt'));
            $(this).val(unit_cost);

            var roundType = $('#hdnDecimalCalType').val();
            var thisMonth = $(this).attr("alt");
            var workDay = getWorkDayOfMonth(thisMonth, thisMonth) ? getWorkDayOfMonth(thisMonth, thisMonth)[0] : 0;
            var planManDay = $('.tb-ma-center tr[id="' + res[0].USER_ID + '"] .ma-man-days[id="' + res[0].USER_ID + '"]').val();
            var unitPrice = $('table.tb-ma-center tr[id="' + res[0].USER_ID + '"] td.td-ma-detail[headers="' + thisMonth + '"] input.ma-unit-cost-in-month').val();
            var planCost = workDay == 0 ? 0 : PMS.utility.roundingDecimal(unitPrice * planManDay / workDay, roundType);

            $('table.tb-ma-sales-center td.td-ma-sales-detail[headers="' + thisMonth + '"] span.plan-cost[id="' + res[0].USER_ID + '"]').text(PMS.utility.convertIntToMoney(planCost));
            $('table.tb-ma-sales-center td.td-ma-sales-detail[headers="' + thisMonth + '"] input.hdnPlanCost[id="' + res[0].USER_ID + '"]').val(planCost);
            resetTotalIndividualSales(res[0].USER_ID);
        })
        resetAllTotalByMonth();
        setSummaryLabels();
        $('#hdnUserChangeValue, #IS_CHANGE_HISTORY').val(true);
    }
}

// Reset name of controls on member list
function resetMemberDetail() {
    var $maDetailArr = $('td.td-ma-detail');
    var maSalesDetailArr = $('input.ma-sales');
    var planCostArr = $('input.hdnPlanCost');
    var $maArr = $('td.td-ma');

    if ($maDetailArr.length > 0) {
        for (var i = 0; i < $maDetailArr.length; i++) {
            var maDetail = $maDetailArr[i];
            var ma_id = 'MEMBER_ASSIGNMENT_DETAIL_LIST[' + i + '].user_sys_id';
            var target_time = 'MEMBER_ASSIGNMENT_DETAIL_LIST[' + i + '].target_time';
            var man_days = 'MEMBER_ASSIGNMENT_DETAIL_LIST[' + i + '].plan_man_days';
            var individual_sales = 'MEMBER_ASSIGNMENT_DETAIL_LIST[' + i + '].individual_sales';
            var unit_cost_in_month = 'MEMBER_ASSIGNMENT_DETAIL_LIST[' + i + '].unit_cost';
            var plan_cost_in_month = 'MEMBER_ASSIGNMENT_DETAIL_LIST[' + i + '].plan_cost';

            $(maDetail).children('.ma-id').attr('name', ma_id);
            $(maDetail).children('.ma-target-time').attr('name', target_time);
            $(maDetail).children('.ma-man-days').attr('name', man_days);
            $(maDetail).children('.ma-unit-cost-in-month').attr('name', unit_cost_in_month);
            $(planCostArr[i]).attr('name', plan_cost_in_month);
            $(maSalesDetailArr[i]).attr('name', individual_sales);
        }
    }

    if ($maArr.length > 0) {
        for (var i = 0; i < $maArr.length; i++) {
            var ma = $maArr[i];
            var ma_id = 'MEMBER_ASSIGNMENT_LIST[' + i + '].user_sys_id';
            //var unit_cost = 'MEMBER_ASSIGNMENT_LIST[' + i + '].unit_cost';
            var total_plan_man_days = 'MEMBER_ASSIGNMENT_LIST[' + i + '].total_plan_man_days';
            var total_plan_cost = 'MEMBER_ASSIGNMENT_LIST[' + i + '].total_plan_cost';

            $(ma).children('.ma-id').attr('name', ma_id);
            //$(ma).children('.unit-cost').attr('name', unit_cost);
            $(ma).children('.ma-total-manday').attr('name', total_plan_man_days);
            $(ma).children('.ma-total-cost').attr('name', total_plan_cost);
        }
    }
}

// Reset name of controls on payment amount list
function resetArrPaymentDetail() {
    var $paymentDetailArr = $('td.td-payment-detail');

    if ($paymentDetailArr.length > 0) {
        for (var i = 0; i < $paymentDetailArr.length; i++) {
            var paymentDetail = $paymentDetailArr[i];
            var customer_id = 'PAYMENT_DETAIL_LIST[' + i + '].customer_id';
            var ordering_flg = 'PAYMENT_DETAIL_LIST[' + i + '].ordering_flg';
            var target_time = 'PAYMENT_DETAIL_LIST[' + i + '].target_time';
            var amount = 'PAYMENT_DETAIL_LIST[' + i + '].amount';

            $(paymentDetail).children('.payment-customer-id').attr('name', customer_id);
            $(paymentDetail).children('.payment-target-time').attr('name', target_time);
            $(paymentDetail).children('.payment-amount').attr('name', amount);
        }
    }
}

// Reset name of controls on overhead cost amount list
function resetArrOverheadCostDetail() {
    var $ovcDetailArr = $('td.td-ovc-content');

    if ($ovcDetailArr.length > 0) {
        for (var i = 0; i < $ovcDetailArr.length; i++) {
            var ovcDetail = $ovcDetailArr[i];
            var ovcId = 'OVERHEAD_COST_DETAIL_LIST[' + i + '].detail_no';
            var target_time = 'OVERHEAD_COST_DETAIL_LIST[' + i + '].target_time';
            var amount = 'OVERHEAD_COST_DETAIL_LIST[' + i + '].amount';

            $(ovcDetail).children('.ovc-id').attr('name', ovcId);
            $(ovcDetail).children('.ovc-target-time').attr('name', target_time);
            $(ovcDetail).children('.ovc-amount').attr('name', amount);
        }
    }
}

// Reset colums when time change
function onChangeTime(timeControl) {
    var startDate = $('#PROJECT_INFO_start_date').val();
    var endDate = $('#PROJECT_INFO_end_date').val();
    var colums = PMS.utility.getMonthCols(startDate, endDate);

    if (startDate.length == 0 || endDate.length == 0)
        return;

    PMS.utility.bindMonthCols(startDate, endDate);

    var $trMA = $('table.tb-ma-center tr:not(.tb-header, .tr-total)');
    var $trMaDetail = $('table.tb-ma-center tr:not(.tb-header, .tr-total)');
    var $trPayment = $('table.tb-sc-left tr:not(.tb-header, .tr-total)');
    var $trPaymentDetail = $('table.tb-sc-center tr:not(.tb-header, .tr-total)');
    var $trOvc = $('table.tb-ovc-left tr:not(.tb-header, .tr-total)');
    var $trOvcDetail = $('table.tb-ovc-center tr:not(.tb-header, .tr-total)');
    var trOld;
    var tdClass;

    if ($trMaDetail.length > 0) {
        trOld = $trMaDetail.first();
        tdClass = '.td-ma-detail';
    } else if ($trPayment.length > 0) {
        trOld = $trPaymentDetail.first();
        tdClass = '.td-payment-detail';
    } else if ($trOvc.length > 0) {
        trOld = $trOvcDetail.first();
        tdClass = '.td-ovc-content';
    } else {
        PMS.utility.bindTotalCols(null, startDate, endDate);
        return;
    }

    var arrData = $(trOld).children(tdClass);

    if (trOld.length === 0)
        return;

    if (arrData.length === colums.length) {
        var isDifferent = false;
        for (var i = 0; i < colums.length; i++) {
            if (colums[i] != $(arrData[i]).attr('headers')) {
                isDifferent = true;
                break;
            }
        }

        if (!isDifferent)
            return;
    }

    if ($trMaDetail.length > 0) {
        var $arrMaDetail = $trMaDetail.children('td.td-ma-detail');
        var $trMaSales = $('table.tb-ma-sales-center tr:not(.tb-header, .tr-total)');
        var $arrMaSalesDetail = $trMaSales.children('td.td-ma-sales-detail');

        $trMaDetail.each(function () {
            var userId = $(this).attr('id');
            var html = '<tr id="' + userId + '">';
            var htmlSales = '<tr id="' + userId + '">';

            for (var i = 0; i < colums.length; i++) {
                var $oldDetail = $arrMaDetail.children('input.ma-man-days[id="' + userId + '"][alt="' + colums[i] + '"]');
                var $oldSales = $arrMaSalesDetail.find('div.div-sales div input.ma-sales[id="' + userId + '"][alt="' + colums[i] + '"]');
                var $oldUnitCost = $arrMaDetail.children('input.ma-unit-cost-in-month[id="' + userId + '"][alt="' + colums[i] + '"]');
                var $oldPlanCost = $arrMaSalesDetail.find(' span.plan-cost[id="' + userId + '"][alt="' + colums[i] + '"]');
                var $oldHdnPlanCost = $arrMaSalesDetail.find(' input.hdnPlanCost[id="' + userId + '"][alt="' + colums[i] + '"]');
                var htmlInput = '';
                var htmlInputSales = '';
                var htmlInputUnitCost = '';
                var htmlInputPlanCost = '';
                var htmlHdnPlanCost = '';

                if ($oldDetail.length > 0) {
                    $oldDetail.attr('value', $oldDetail.val());
                    $oldSales.attr('value', $oldSales.val());
                    $oldUnitCost.attr('value', $oldUnitCost.val());
                    $oldPlanCost.text($oldPlanCost.text() === '' ? 0 : $oldPlanCost.text());
                    $oldHdnPlanCost.attr('value', $oldHdnPlanCost.val());
                    htmlInput = $oldDetail.prop('outerHTML');
                    htmlInputSales = $oldSales.prop('outerHTML');
                    htmlInputUnitCost = $oldUnitCost.prop('outerHTML');
                    htmlInputPlanCost = $oldPlanCost.prop('outerHTML');
                    htmlHdnPlanCost = $oldHdnPlanCost.prop('outerHTML');
                } else {
                    var param = { userId: PMS.utility.nvl(userId) };
                    var listUnitCost = PMS.utility.getDataByAjax('/PMS01002/GetListUnitCost', param);

                    var unitCostOfMonth = getUnitCostOfMonth(listUnitCost, colums[i])
                    htmlInput = ' <input id="' + userId + '" class="ma-man-days right decimal valid w80" type="text" alt="' + colums[i] + '" maxlength="6">';
                    htmlInputSales = ' <input id="' + userId + '" class="ma-sales money right numeric valid w80" type="text" alt="' + colums[i] + '" maxlength="9">';
                    htmlInputUnitCost = ' <input class="ma-unit-cost-in-month" type="hidden" id="' + userId + '" alt="' + colums[i] + '" value ="' + unitCostOfMonth + '">';
                    htmlHdnPlanCost = '<input type ="hidden" id="' + userId + '" class="hdnPlanCost" alt="' + colums[i] + '" value="0">';
                    htmlInputPlanCost = '<span id="' + userId + '" class="font-normal plan-cost" alt="' + colums[i] + '">0</span>'
                }

                html += ' <td class="td-ma-detail right" headers="' + colums[i] + '">'
                    + htmlInputUnitCost
                    + ' <input class="ma-id" type="hidden" value="' + userId + '">'
                    + ' <input class="ma-target-time" type="hidden" value="' + colums[i] + '">'
                    + htmlInput
                    + ' <label> 人日</label></td>';
                htmlSales += '<td class="td-ma-sales-detail" headers="' + colums[i] + '">'
                    + '<div class="div-sales"><div>' + htmlInputSales
                    + ' 円 /</div>'
                    + '<div>' + htmlHdnPlanCost + htmlInputPlanCost + '円</div></div>'
            }
            html += '</tr>';
            htmlSales += '</tr>';

            $('table.tb-ma-center tr.tr-total').before(html);
            $('table.tb-ma-sales-center tr.tr-total').before(htmlSales);
        });

        $trMaDetail.remove();
        $trMaSales.remove();
        resetMemberDetail();
    }

    if ($trPayment.length > 0) {
        var $arrPaymentDetail = $trPaymentDetail.children('td.td-payment-detail');

        $trPaymentDetail.remove();

        $trPayment.children('td.td-payment-name').each(function () {
            var scId = $(this).parent().attr('id');
            var html = '<tr id="' + scId + '">';

            for (var i = 0; i < colums.length; i++) {
                var $oldDetail = $arrPaymentDetail.children('input.payment-amount[id="' + scId + '"][alt="' + colums[i] + '"]');
                var htmlInput = '';

                if ($oldDetail.length > 0) {
                    $oldDetail.attr('value', $oldDetail.val());
                    htmlInput = $oldDetail.prop('outerHTML');
                } else
                    htmlInput = ' <input class="money payment-amount right numeric valid w80" maxlength="9" type="text" id="' + scId + '" alt="' + colums[i] + '">'

                html += ' <td class="td-payment-detail right" headers="' + colums[i] + '">'
                    + ' <input class="payment-customer-id" type="hidden" value="' + scId + '">'
                    + ' <input class="payment-target-time" type="hidden" value="' + colums[i] + '">'
                    + htmlInput
                    + ' <label> 円</label></td>';
            }
            html += '</tr>';

            $('table.tb-sc-center tr.tr-total').before(html);
            resetTotalPaymentDetail(scId);
        });
        resetArrPaymentDetail();
    }

    if ($trOvc.length > 0) {
        var $arrOvcDetail = $trOvcDetail.children('td.td-ovc-content');

        $trOvcDetail.remove();

        $trOvc.each(function () {
            var ovcId = $(this).attr('id');
            var html = '<tr id="' + ovcId + '">';

            for (var i = 0; i < colums.length; i++) {
                var $oldDetail = $arrOvcDetail.children('input.ovc-amount[id="' + ovcId + '"][alt="' + colums[i] + '"]');
                var htmlInput = '';

                if ($oldDetail.length > 0) {
                    $oldDetail.attr('value', $oldDetail.val());
                    htmlInput = $oldDetail.prop('outerHTML');
                } else
                    htmlInput = ' <input class="money ovc-amount right numeric valid w80" type="text" id="' + ovcId + '" alt="' + colums[i] + '">'

                html += ' <td class="td-ovc-content right" headers="' + colums[i] + '">'
                    + ' <input class="ovc-id" type="hidden" value="' + ovcId + '">'
                    + ' <input class="ovc-target-time" type="hidden" value="' + colums[i] + '">'
                    + htmlInput
                    + ' <label> 円</label></td>';
            }
            html += '</tr>';

            $('table.tb-ovc-center tr.tr-total').before(html);
            resetTotalOvcDetail(ovcId);
        });
        resetArrOverheadCostDetail();
    }

    setSummaryLabels();
    PMS.utility.bindTotalCols(colums, null, null);
    resetAllTotalByMonth();
    setTotalIndividualSales();
}

// Set total individual sales
function setTotalIndividualSales() {
    $('input.ma-sales').each(function () {
        resetTotalIndividualSales($(this).attr('id'));
    });
}

// Reset total individual sales of a member when has changed
function resetTotalIndividualSales(maId) {
    var totalIndividualSales = 0;
    var planCostByUser = 0;

    $('input.ma-sales[id="' + maId + '"]').each(function () {
        var amount = PMS.utility.convertMoneyToInt($(this).val());

        totalIndividualSales += amount;
    });

    $('table.tb-ma-sales-right label[id="' + maId + '"]').text(PMS.utility.convertIntToMoney(totalIndividualSales));

    // Set total plan cost
    $('span.plan-cost[id="' + maId + '"]').each(function () {
        var planCost = PMS.utility.convertMoneyToInt($(this).text());

        planCostByUser += planCost;
    });

    $('table.tb-ma-sales-right label.plan-cost-by-user[id="' + maId + '"]').text(PMS.utility.convertIntToMoney(planCostByUser));
}

// Reset total payment of a subcontractor when has changed
function resetTotalPaymentDetail(scID) {
    var totalPayment = 0;

    $('input.payment-amount[id="' + scID + '"]').each(function () {
        var amount = PMS.utility.convertMoneyToInt($(this).val());

        totalPayment += amount;
    });
    var $control = $('table.tb-sc-right label[id="' + scID + '"]');
    $control.text(PMS.utility.convertIntToMoney(totalPayment));

    var payment = $('.sc-id[value="' + scID + '"]').siblings('.sc-payment').val();

    if (totalPayment != PMS.utility.convertMoneyToInt(payment))
        $control.addClass('error-compare');
    else
        $control.removeClass('error-compare');
}

// Reset total amount of a overhead cost when has changed
function resetTotalOvcDetail(ovcID) {
    var totalPayment = 0;

    $('input.ovc-amount[id="' + ovcID + '"]').each(function () {
        var amount = PMS.utility.convertMoneyToInt($(this).val());

        totalPayment += amount;
    });

    var $control = $('table.tb-ovc-right label[id="' + ovcID + '"]');

    $control.text(PMS.utility.convertIntToMoney(totalPayment));

    var payment = $('.ovc-id[value="' + ovcID + '"]').siblings('.ovc-total-amount').val();

    if (totalPayment != PMS.utility.convertMoneyToInt(payment))
        $control.addClass('error-compare');
    else
        $control.removeClass('error-compare');
}

// Reset name of dropdownlist on target category list
function resetArrCategory() {
    var $arrCategory = $('div.category-content');

    for (var i = 0; i < $arrCategory.length; i++) {
        var targetCategory = $arrCategory[i];
        var category = 'TARGET_CATEGORY_LIST[' + i + '].category_id';
        var subCategory = 'TARGET_CATEGORY_LIST[' + i + '].sub_category_id';

        $(targetCategory).children('.ddlCategory').attr('name', category);
        $(targetCategory).children('.ddlSubCategory').attr('name', subCategory);
    }
}

// Set value for dropdownlist on target category list
function setArrCategory() {
    var $arrCategory = $('div.category-content');

    $arrCategory.each(function () {
        var categoryId = $(this).children('#hdnCategory').val() == '0' ? '' : $(this).children('#hdnCategory').val();

        $(this).children('.ddlCategory').val(categoryId);

        if (categoryId.length > 0) {
            var subCategoryList = getSubCategoryList(categoryId);
            var $ddlSubCategory = $(this).children('.ddlSubCategory');

            $ddlSubCategory.empty();

            if (subCategoryList.length > 0) {
                $ddlSubCategory.append('<option value="">' + Constant.NONE_SPECIFIED + '</option>');

                $.each(subCategoryList, function (i, item) {
                    $ddlSubCategory.append('<option value="' + item.sub_category_id + '">' + PMS.utility.htmlEncode(item.sub_category) + '</option>');
                });
                $ddlSubCategory.val($(this).children('#hdnSubCategory').val());
            }
        }
    });
}

// Get data for sub category dropdownlist on target category list
function getSubCategoryList(categoryId) {
    var param = { categoryId: categoryId, projectID: parseInt($('#PROJECT_INFO_project_sys_id').val()) };
    var subCategoryList = PMS.utility.getDataByAjax('/PMS06001/SubCategoryListJson', param);

    return subCategoryList;
}

// Get number work day of the month
function getWorkDayOfMonth(start, end) {
    var startArr = start.split('/');
    var endArr = end.split('/');
    var param = {
        fromYear: startArr[0],
        fromMonth: startArr[1],
        toYear: endArr[0],
        toMonth: endArr[1]
    };

    var workDayOfMonth = PMS.utility.getDataByAjax('/PMS06001/WorkDayOfMonthJson', param);

    return workDayOfMonth;
}

// Get actual work day of member
function getMemberActualWorkDay(userID, start, end) {
    var startArr = start.split('/');
    var endArr = end.split('/');
    var param = {
        projectID: $('#PROJECT_INFO_project_sys_id').val(),
        userID: userID,
        fromYear: startArr[0],
        fromMonth: startArr[1],
        toYear: endArr[0],
        toMonth: endArr[1]
    };

    var data = PMS.utility.getDataByAjax('/PMS06001/GetMemberActualWorkDayJson', param);

    return data;
}

// Set summary label at bottom of form
function setSummaryLabels() {
    var roundType = $('#hdnDecimalCalType').val();
    var tax = $('#PROJECT_INFO_tax_rate').val();
    var taxRate = PMS.utility.validPositiveNumeric(tax) ? parseInt(tax) / 100 + 1 : 1;
    var intTotalSales = PMS.utility.convertMoneyToInt($('#PROJECT_INFO_total_sales').val());
    var strTotalSales = PMS.utility.convertIntToMoney(intTotalSales);
    var intTotalPayment = PMS.utility.convertMoneyToInt($('#PROJECT_INFO_total_payment').val().length > 0 ? $('#PROJECT_INFO_total_payment').val() : '0');
    var strTotalPayment = PMS.utility.convertIntToMoney(intTotalPayment);
    var totalSalesIncTax = PMS.utility.convertIntToMoney(PMS.utility.roundingDecimal(intTotalSales * taxRate, roundType));
    var totalPaymentIncTax = PMS.utility.convertIntToMoney(PMS.utility.roundingDecimal(intTotalPayment * taxRate, roundType));
    var totalPlanCost = 0;
    var totalActualCost = 0;

    var $thMonth = $('table.tb-ma-center tr.tb-header th');

    if ($thMonth.length > 0) {
        var fromDate = $thMonth.first().html();
        var toDate = $thMonth.last().html();

        var arrWorkDayOfMonth = getWorkDayOfMonth(fromDate, toDate);

        $('td.td-ma').each(function () {
            var totalCostPerMember = 0;
            var totalPlan = 0;
            var userId = $(this).children('.ma-id').val();
            var actualWorkDays = getMemberActualWorkDay(userId, fromDate, toDate);

            $('table.tb-ma-center tr[id="' + userId + '"] td.td-ma-detail').each(function (index) {
                var workDay = parseInt(arrWorkDayOfMonth[index]);
                var unitPrice = PMS.utility.convertMoneyToInt($(this).children('.ma-unit-cost-in-month').val());

                // calculate actual
                var planManDay = PMS.utility.validDecimalNumeric($(this).children('.ma-man-days').val()) ? parseFloat($(this).children('.ma-man-days').val()) : 0;
                var planCost = workDay == 0 ? 0 : PMS.utility.roundingDecimal(unitPrice * planManDay / workDay, roundType);

                // calculate actual
                var clientMonth = $(this).attr('headers');

                totalPlan += planManDay;
                totalCostPerMember += planCost;
                totalPlanCost += planCost;


                for (var i = 0; i < actualWorkDays.length; i++) {
                    var serverMonth = actualWorkDays[i].actual_work_year + '/' + (actualWorkDays[i].actual_work_month.toString().length == 1 ? '0' + actualWorkDays[i].actual_work_month.toString() : actualWorkDays[i].actual_work_month);

                    if (clientMonth == serverMonth) {
                        var actualCost = workDay == 0 ? 0 : PMS.utility.roundingDecimal(unitPrice * actualWorkDays[i].actual_work_time / workDay, roundType);
                        totalActualCost += actualCost;
                    }
                }
            });

            $(this).children('.ma-total-manday').val(totalPlan.toFixed(1));
            $(this).children('.ma-total-cost').val(totalCostPerMember);
            $('table.tb-ma-right label[id="' + userId + '"]').text(totalPlan.toFixed(1));
        });
    }

    totalPlanCost = PMS.utility.roundingDecimal(totalPlanCost, roundType);
    totalActualCost = PMS.utility.roundingDecimal(totalActualCost, roundType);

    var expectedGrossProfit = PMS.utility.roundingDecimal(intTotalSales - intTotalPayment - totalPlanCost, roundType);
    var expectedGrossMargin = (expectedGrossProfit / intTotalSales) * 100;
    var strExpectedGrossMargin = $.isNumeric(expectedGrossMargin) ? expectedGrossMargin.toFixed(2) + '%' : '0.00%';

    var actualProfit = PMS.utility.roundingDecimal(intTotalSales - intTotalPayment - totalActualCost, roundType);
    var actualProfitRate = (actualProfit / intTotalSales) * 100;
    var strActualProfitRate = $.isNumeric(actualProfitRate) ? actualProfitRate.toFixed(2) + '%' : '0.00%';

    setTimeout(function () {
        $('#lblTotalPayment').text(strTotalPayment + '円');
        $('#lblTotalPaymentIncTax').text(totalPaymentIncTax + '円');
        $('#lblTotalSales').text(strTotalSales + '円');
        $('#lblTotalSalesIncTax').text(totalSalesIncTax + '円');
        $('#lblTotalCost').text(PMS.utility.convertIntToMoney(totalPlanCost) + '円');
        $('#lblExpectedGrossProfit').text(PMS.utility.convertIntToMoney(expectedGrossProfit) + '円');
        $('#lblExpectedGrossMargin').text(strExpectedGrossMargin);
        $('#lblActualProfit').text(PMS.utility.convertIntToMoney(actualProfit) + '円');
        $('#lblActualProfitRate').text(strActualProfitRate);
        $('#PROJECT_INFO_gross_profit').val(expectedGrossProfit);
    }, 1);
}

// set estimate summary label at bottom of form
function setEstimateLabel() {
    var value = $('#PROJECT_INFO_estimate_man_days').val().trim();
    var estimate = PMS.utility.validDecimalNumeric(value) ? value + '人日' : '0.0人日';

    $('#lblEstimate').text(estimate);
}

// Reset tax rate value
function resetTaxRate(startDate) {
    var param = { fromDate: startDate };
    var data = PMS.utility.getDataByAjax('/PMS06001/TaxRateJson', param);
    var $taxRate = $('#PROJECT_INFO_tax_rate');

    if ($('#PROJECT_INFO_project_sys_id').val() === '0' && !$taxRate.hasClass('changed')) {
        $('#PROJECT_INFO_tax_rate').val(data);
        $taxRate.addClass('changed');
    }

    if (parseInt($taxRate.val()) !== data) {
        PMS.utility.showSubmitConfirmDialog(Constant.CONFIRM_CHANGE_TAX_RATE, null, null, function (action) {
            if (action) {
                $('#PROJECT_INFO_tax_rate').val(data);
                BootstrapDialog.closeAll();
            }
        });
    }
}

// Reset data list by start date
function resetDataByStartDate(startDate) {
    var endDate = $('#PROJECT_INFO_end_date').val();
    var errInvalidEndDate = PMS.utility.validDate(endDate, Constant.DATE_FORMAT, Constant.PERIOD_END);

    if (errInvalidEndDate != null)
        return;

    var arrStartDate = startDate.split('/');
    var shortStartDate = arrStartDate[0] + '/' + arrStartDate[1];
    var shortOldStartDate = $('#hdnTempStart').val().split('/')[0] + '/' + $('#hdnTempStart').val().split('/')[1];

    if (shortStartDate === shortOldStartDate)
        return;

    if (PMS.utility.compareDate(shortOldStartDate, shortStartDate, Constant.SHORT_DATE_FORMAT)
        && $('table.tb-detail th.th-month').length > 0
        && $('table.tb-detail tr:not(.tb-header, .tr-total)').length > 0
        && endDate.length > 0) {

        var projectId = $('#PROJECT_INFO_project_sys_id').val();

        if (projectId !== '0') {
            if (checkExistActualWorkTimeByMonthYear(projectId)) {
                PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_CHANGE_DURATION_TIME);
                $('#PROJECT_INFO_start_date').val($('#hdnTempStart').val());
                return;
            }
        }

        BootstrapDialog.show({
            title: '<i class="fa fa-warning"></i>',
            message: Constant.CONFIRM_CHANGE_COLUM,
            closable: false,
            buttons: [{
                label: 'OK',
                hotkey: 13,
                cssClass: 'btn dark',
                action: function (dialog) {
                    $('#hdnTempStart').val(startDate);
                    $('#hdnOldDuration').attr('changed', 'true');
                    onChangeTime(Constant.PERIOD_START);
                    resetTaxRate(startDate);
                    dialog.close();
                }
            }, {
                label: 'キャンセル',
                cssClass: 'btn light',
                action: function (dialog) {
                    $('#PROJECT_INFO_start_date').val($('#hdnTempStart').val());
                    resetTaxRate(startDate);
                    dialog.close();
                }
            }]
        });
    } else {
        $('#hdnTempStart').val(startDate);
        $('#hdnOldDuration').attr('changed', 'true');
        onChangeTime(Constant.PERIOD_START);
        resetTaxRate(startDate);
    }
}

// Delete member assignment
function deleteMemberAssignment(maId) {
    if (!$('table.tb-ma-left tr[id="' + maId + '"]').hasClass('new-data'))
        $('#IS_CHANGE_HISTORY, #IS_UPDATE_ASSIGN_DATE').val(true);

    $('table.tb-ma-left tr[id="' + maId + '"], table.tb-ma-center tr[id="' + maId + '"],  table.tb-ma-right tr[id="' + maId + '"]').remove();
    $('table.tb-ma-sales-left tr[id="' + maId + '"], table.tb-ma-sales-center tr[id="' + maId + '"], table.tb-ma-sales-right tr[id="' + maId + '"]').remove();
    $('div[data-id="' + maId + '"]').parent().remove();

    resetMemberDetail();
    setSummaryLabels();
}

// Reset calculate total payment by month
function resetTotalPaymentByMonth() {
    var colums = PMS.utility.getMonthCols($('#PROJECT_INFO_start_date').val(), $('#PROJECT_INFO_end_date').val())
    var payment = 0;

    for (var i = 0; i < colums.length; i++) {
        var paymentByMonth = 0;

        $('input.payment-amount[alt="' + colums[i] + '"]').each(function () {
            var ovcAmount = PMS.utility.convertMoneyToInt($(this).val());

            paymentByMonth += ovcAmount;
        });
        payment += paymentByMonth;
        $('table.tb-sc-center label.font-normal[name="' + colums[i] + '"]').text(PMS.utility.convertIntToMoney(paymentByMonth));
    }

    $('table.tb-sc-right tr.tr-total label.font-normal').text(PMS.utility.convertIntToMoney(payment));
}

// Reset calculate total overheadcost by month
function resetTotalOverheadCostByMonth() {
    var colums = PMS.utility.getMonthCols($('#PROJECT_INFO_start_date').val(), $('#PROJECT_INFO_end_date').val())
    var ovcTotal = 0;

    for (var i = 0; i < colums.length; i++) {
        var ovcTotalByMonth = 0;

        $('input.ovc-amount[alt="' + colums[i] + '"]').each(function () {
            var ovcAmount = PMS.utility.convertMoneyToInt($(this).val());

            ovcTotalByMonth += ovcAmount;
        });
        ovcTotal += ovcTotalByMonth;
        $('table.tb-ovc-center label.font-normal[name="' + colums[i] + '"]').text(PMS.utility.convertIntToMoney(ovcTotalByMonth));
    }

    $('table.tb-ovc-right tr.tr-total label.font-normal').text(PMS.utility.convertIntToMoney(ovcTotal));
}

// Bind data total by month on table list
function resetAllTotalByMonth() {
    var colums = PMS.utility.getMonthCols($('#PROJECT_INFO_start_date').val(), $('#PROJECT_INFO_end_date').val())
    var totalEstimate = 0;
    var totalIndividualSales = 0;
    var totalPlanCost = 0;
    var totalIndividualSalesIncTax = 0;
    var totalPayment = 0;
    var ovcTotal = 0;

    for (var i = 0; i < colums.length; i++) {
        var totalEstimateByMonth = 0;
        var totalIndividualSalesByMonth = 0;
        var totalPlanCostByMonth = 0;
        var totalPaymentByMonth = 0;
        var ovcTotalByMonth = 0;

        $('input.ma-man-days[alt="' + colums[i] + '"]').each(function () {
            var planManDay = PMS.utility.validDecimalNumeric($(this).val()) ? parseFloat($(this).val()) : 0;

            totalEstimateByMonth += planManDay;
        });
        totalEstimate += totalEstimateByMonth;

        $('table.tb-ma-center label.font-normal[name="' + colums[i] + '"]').text(totalEstimateByMonth.toFixed(1));

        $('input.ma-sales[alt="' + colums[i] + '"]').each(function () {
            var individualSales = PMS.utility.convertMoneyToInt($(this).val());

            totalIndividualSalesByMonth += individualSales;
        });

        totalIndividualSales += totalIndividualSalesByMonth;

        $('table.tb-ma-sales-center label.font-normal[name="' + colums[i] + '"]').text(PMS.utility.convertIntToMoney(totalIndividualSalesByMonth));

        $('td.td-ma-sales-detail[headers="' + colums[i] + '"] span.plan-cost').each(function () {
            var planCost = PMS.utility.convertMoneyToInt($(this).text());

            totalPlanCostByMonth += planCost;
        });

        totalPlanCost += totalPlanCostByMonth;

        $('table.tb-ma-sales-center label.plan-cost-by-month[name="' + colums[i] + '"]').text(PMS.utility.convertIntToMoney(totalPlanCostByMonth));

        $('input.payment-amount[alt="' + colums[i] + '"]').each(function () {
            var paymentAmount = PMS.utility.convertMoneyToInt($(this).val());

            totalPaymentByMonth += paymentAmount;
        });
        totalPayment += totalPaymentByMonth;
        $('table.tb-sc-center label.font-normal[name="' + colums[i] + '"]').text(PMS.utility.convertIntToMoney(totalPaymentByMonth));

        $('input.ovc-amount[alt="' + colums[i] + '"]').each(function () {
            var ovcAmount = PMS.utility.convertMoneyToInt($(this).val());

            ovcTotalByMonth += ovcAmount;
        });
        ovcTotal += ovcTotalByMonth;
        $('table.tb-ovc-center label.font-normal[name="' + colums[i] + '"]').text(PMS.utility.convertIntToMoney(ovcTotalByMonth));
    }

    $('table.tb-ma-right tr.tr-total label.font-normal, .total-estimate-man-days').text(totalEstimate.toFixed(1));
    $('#lblEstimate').text(totalEstimate.toFixed(1) + '人日');
    $('#PROJECT_INFO_estimate_man_days').val(totalEstimate.toFixed(1));
    $('table.tb-ma-sales-right tr.tr-total label.font-normal').text(PMS.utility.convertIntToMoney(totalIndividualSales));
    $('#total_individual_sales').text(PMS.utility.convertIntToMoney(totalIndividualSales));
    $('table.tb-ma-sales-right tr.tr-total label.total-plan-cost').text(PMS.utility.convertIntToMoney(totalPlanCost));


    //add data to summary
    $('#hdnTotalOrder').val(totalIndividualSales);
    $('#lblTotalOrder').text(PMS.utility.convertIntToMoney(totalIndividualSales) + '円');
    var tax = $('#PROJECT_INFO_tax_rate').val();
    var taxRate = PMS.utility.validPositiveNumeric(tax) ? parseInt(tax) / 100 + 1 : 1;
    var roundType = $('#hdnDecimalCalType').val();
    totalIndividualSalesIncTax = PMS.utility.convertIntToMoney(PMS.utility.roundingDecimal(totalIndividualSales * taxRate, roundType));
    $('#lblTotalOrderIncTax').text(PMS.utility.convertIntToMoney(totalIndividualSalesIncTax) + '円');
    $('table.tb-sc-right tr.tr-total label.font-normal').text(PMS.utility.convertIntToMoney(totalPayment));
    $('table.tb-ovc-right tr.tr-total label.font-normal').text(PMS.utility.convertIntToMoney(ovcTotal));
}

// Reset index of element in subcontractor array
function resetArrProgress() {
    $progressContentArr = $('.progress-list .tb-detail tr');

    for (var i = 0; i < $progressContentArr.length; i++) {
        var progressContent = $progressContentArr[i];
        var registDate = 'PROGRESS_LIST[' + i + '].regist_date';
        var progress = 'PROGRESS_LIST[' + i + '].progress';
        var remarks = 'PROGRESS_LIST[' + i + '].remarks';
        var isdelete = 'PROGRESS_LIST[' + i + '].isDelete';
        var isnew = 'PROGRESS_LIST[' + i + '].isNew';

        $(progressContent).find('.hdnProgressRegistDate').attr('name', registDate);
        $(progressContent).find('.hdnProgressPercent').attr('name', progress);
        $(progressContent).find('.hdnProgressRemarks').attr('name', remarks);
        $(progressContent).find('.hdnIsDelete').attr('name', isdelete);
        $(progressContent).find('.hdnIsNew').attr('name', isnew);
    }
}

// Check exist actual work time by phase
function checkExistActualWorkTimeByPhase(dataSend) {
    var data = PMS.utility.getDataByAjax('/PMS06001/CheckDeletePhase', dataSend);
    var isExist = false;

    if (data.actual_work_time > 0)
        isExist = true;

    return isExist;
}

// Check exist actual work time by month year
function checkExistActualWorkTimeByMonthYear(projectId) {
    var isExist = false;
    var colums = PMS.utility.getMonthCols($('#PROJECT_INFO_start_date').val(), $('#PROJECT_INFO_end_date').val())
    var arrData = $('table.tb-ma-center tr th');
    var deleteColsArr = [];

    for (var i = 0; i < arrData.length; i++) {
        var time = $(arrData[i]).text();

        if (colums.indexOf(time) == -1)
            deleteColsArr.push(time);
    }

    if (deleteColsArr.length > 0) {
        var param = {
            projectId: projectId,
            timeArr: deleteColsArr
        }
        var data = PMS.utility.checkDataExistByAjax('/PMS06001/CheckChangeDuration', param);

        if (typeof (data) !== 'undefined' && data.actual_work_time > 0)
            isExist = true;
    }

    return isExist;
}

// load data on table list
function loadDataOnTableList(projectID, startDate, endDate) {
    var param = { projectID: projectID, startDate: startDate, endDate: endDate };
    var data = PMS.utility.getDataByAjax('/PMS06001/GetDataOnTableList', param);
    var colums = PMS.utility.getMonthCols(startDate, endDate);
    var colLength = colums.length;
    var isCopy = $("#COPY_TYPE").val() == Constant.COPY_PRJ_TYPE.ALL_INFORMATION ? true : false
    var roundType = $('#hdnDecimalCalType').val();
    var arrWorkDayOfMonth = getWorkDayOfMonth(startDate, endDate);

    if (data.memberAssignments.length > 0) {
        var htmlMaLeft = '';
        var htmlMaSalesLeft = '';
        var htmlMaCenter = '';
        var htmlMaSalesCenter = '';
        var htmlMaRight = '';
        var htmlMaSalesRight = '';
        var htmlMaContent = '';

        for (var i = 0; i < data.memberAssignments.length; i++) {
            var maName = PMS.utility.htmlEncode(PMS.utility.nvl(data.memberAssignments[i][0].Value, ""));
            var maGroupName = PMS.utility.htmlEncode(PMS.utility.nvl(data.memberAssignments[i][1].Value, ""));
            var maID = data.memberAssignments[i][2].Value;

            htmlMaLeft += '<tr id="' + maID + '">'
                + ' <td class="td-ma"><div class="ma-display-name text-overflow" title="' + PMS.utility.htmlEncodeByReplace(maName) + '">' + PMS.utility.htmlEncodeByReplace(maName) + '</div>'
                //+ ' <td class="td-ma right">'
                + ' <input class="ma-id" type="hidden" value="' + maID + '" name="MEMBER_ASSIGNMENT_LIST[' + i + '].user_sys_id">'
                + ' <input alt="' + maID + '" class="ma-total-manday" name="MEMBER_ASSIGNMENT_LIST[' + i + '].total_plan_man_days" type="hidden" value="' + data.memberAssignments[i][4].Value + '">'
                + ' <input alt="' + maID + '" class="ma-total-cost" name="MEMBER_ASSIGNMENT_LIST[' + i + '].total_plan_cost" type="hidden" value="' + data.memberAssignments[i][5].Value + '">'
                //+ ' <input class="money unit-cost right numeric valid w80" type="text" name="MEMBER_ASSIGNMENT_LIST[' + i + '].unit_cost" maxlength="9" value="' + data.memberAssignments[i][3].Value + '" oldValue="' + data.memberAssignments[i][3].Value + '" alt="' + maID + '">'
                //+ ' 円'
                + '</td></tr> ';
            htmlMaSalesLeft += '<tr id="' + maID + '">'
                + ' <td> <div class="ma-sales-display-name text-overflow" title="' + PMS.utility.htmlEncodeByReplace(maName) + '">' + PMS.utility.htmlEncodeByReplace(maName) + '</div> </td>'
                + ' </tr>';
            htmlMaCenter += ' <tr id="' + maID + '">';
            htmlMaSalesCenter += ' <tr id="' + maID + '">';

            if (!isCopy) {
                for (var j = 0; j < colLength; j++) {
                    var index = i * colLength + j;
                    var dataDetail = data.memberAssignments[i][j + 5].Value.split('/');
                    var workDay = arrWorkDayOfMonth[j];

                    var planCost = workDay == 0 ? 0 : PMS.utility.roundingDecimal(dataDetail[2] * parseFloat(dataDetail[0]).toFixed(1) / workDay, roundType);

                    htmlMaCenter += '<td class="td-ma-detail right" headers="' + colums[j] + '">'
                        + '<input class="ma-unit-cost-in-month" type="hidden" name="MEMBER_ASSIGNMENT_DETAIL_LIST[' + index + '].unit_cost" id="' + maID + '" alt="' + colums[j] + '" value ="' + dataDetail[2] + '">'
                        + '<input class="ma-id" type="hidden" name="MEMBER_ASSIGNMENT_DETAIL_LIST[' + index + '].user_sys_id" value="' + maID + '">'
                        + '<input class="ma-target-time" name="MEMBER_ASSIGNMENT_DETAIL_LIST[' + index + '].target_time" type="hidden" value="' + colums[j] + '">'
                        + '<input id="' + maID + '" class="ma-man-days right decimal valid w80" name="MEMBER_ASSIGNMENT_DETAIL_LIST[' + index + '].plan_man_days" type="text" alt="' + colums[j] + '" maxlength="6" value="' + parseFloat(dataDetail[0]).toFixed(1) + '" oldValue="' + parseFloat(dataDetail[0]).toFixed(1) + '" >'
                        + ' 人日</td>';
                    htmlMaSalesCenter += '<td class="td-ma-sales-detail" headers="' + colums[j] + '">'
                        + '<div class="div-sales"><div><input id="' + maID + '" name="MEMBER_ASSIGNMENT_DETAIL_LIST[' + index + '].individual_sales" class="ma-sales money right numeric valid w80" type="text" alt="' + colums[j] + '" maxlength="9" value="' + dataDetail[1] + '" oldValue="' + dataDetail[1] + '">'
                        + ' 円 /</div><div><input type ="hidden" id="' + maID + '" name="MEMBER_ASSIGNMENT_DETAIL_LIST[' + index + '].plan_cost" class="hdnPlanCost" alt="' + colums[j] + '" value="' + planCost + '"><span id="' + maID + '" class="font-normal plan-cost" alt="' + colums[j] + '">' + PMS.utility.convertIntToMoney(planCost) + '</span>円</div></div></td>';
                }
            }

            htmlMaCenter += ' </tr>';
            htmlMaSalesCenter += ' </tr>';
            htmlMaRight += '<tr id="' + maID + '">'
                + ' <td class="right"> <label class="font-normal" id="' + maID + '">' + (isCopy ? '0' : data.memberAssignments[i][4].Value) + '</label> <label>人日</label> </td>'
                + ' </tr>';
            htmlMaSalesRight += '<tr id="' + maID + '">'
                + ' <td class="right"><div class="div-sales"><div><label class="font-normal lbl-money ma-sales-by-user" id="' + maID + '">0</label> <label>円 /&nbsp;</label></div><div><label id="' + maID + '" class="font-normal plan-cost-by-user"></label><label>&nbsp;円</label></div></div></td>'
                + ' </tr>';

            if (maID != $('#PROJECT_INFO_charge_person_id').val()) {
                htmlMaContent += '<div class="form-group no-bottom col-sm-12 member-content">'
                    + ' <label class="col-sm-2 bold">メンバー</label>'
                    + ' <div class="col-sm-9 col-input mar-left" data-id="' + maID + '">'
                    + ' <input class="value-selected" placeholder="所属" readonly="readonly" title="' + PMS.utility.htmlEncodeByReplace(maGroupName) + '" type="text" value="' + PMS.utility.htmlEncodeByReplace(maGroupName) + '">'
                    + ' <input class="value-selected" placeholder="名前" readonly="readonly" title="' + PMS.utility.htmlEncodeByReplace(maName) + '" type="text" value="' + PMS.utility.htmlEncodeByReplace(maName) + '">'
                    + ' <button type="button" class="btn light btnChangeMember"><i class="btn-icon btn-search-dialog"></i></button>'
                    + ' <label class="lbl-action btnDeleteMa">削除</label>'
                    + ' </div>'
                    + ' </div>';
            }
        }

        if ($('.sales-pic-content').length == 0) {
            $('.pic-content').after(htmlMaContent);
        } else {
            $('.sales-pic-content').after(htmlMaContent);
        }

        $('table.tb-ma-left tr.tr-total').before(htmlMaLeft);
        $('table.tb-ma-sales-left tr.tr-total').before(htmlMaSalesLeft);
        $('table.tb-ma-center tr.tr-total').before(htmlMaCenter);
        $('table.tb-ma-sales-center tr.tr-total').before(htmlMaSalesCenter);
        $('table.tb-ma-right tr.tr-total').before(htmlMaRight);
        $('table.tb-ma-sales-right tr.tr-total').before(htmlMaSalesRight);

        setTotalIndividualSales();
        PMS.utility.imeControl($(".unit-cost, .ma-man-days, .ma-sales"), 'disable');
    }

    if (data.paymentDetails.length > 0) {
        var htmlCenter = '';
        var htmlRight = '';

        for (var i = 0; i < data.paymentDetails.length; i++) {
            var totalPaymentAmount = 0;
            var customerID = data.paymentDetails[i][0].Value;

            htmlCenter += '<tr id="' + customerID + '">';
            htmlRight += '<tr id="' + customerID + '">';

            if (!isCopy) {
                for (var j = 0; j < colLength; j++) {
                    var index = i * colLength + j;
                    var paymentAmount = data.paymentDetails[i][j + 2].Value;

                    htmlCenter += ' <td class="td-payment-detail right" headers="' + colums[j] + '">'
                        + ' <input name="PAYMENT_DETAIL_LIST[' + index + '].customer_id" class="payment-customer-id" type="hidden" value="' + customerID + '">'
                        + ' <input name="PAYMENT_DETAIL_LIST[' + index + '].target_time" class="payment-target-time" type="hidden" value="' + colums[j] + '">'
                        + ' <input name="PAYMENT_DETAIL_LIST[' + index + '].amount" class="money payment-amount right numeric valid w80" maxlength="9" type="text" id="' + customerID + '"  alt="' + colums[j] + '" value="' + paymentAmount + '" oldValue="' + paymentAmount + '">'
                        + ' 円</td>';

                    totalPaymentAmount += parseInt(paymentAmount);
                }
            }

            var classCompare = totalPaymentAmount != PMS.utility.convertMoneyToInt($('.sc-id[value="' + customerID + '"]').siblings('.sc-payment').val()) && !isCopy ? 'error-compare' : '';

            htmlCenter += ' </tr>';
            htmlRight += ' <td class="right">'
                + ' <label class="font-normal lbl-money ' + classCompare + '" id="' + customerID + '">' + (isCopy ? '0' : totalPaymentAmount) + '</label>'
                + ' <label>円</label>'
                + ' </td>'
                + ' </tr>';
        }

        $('table.tb-sc-center tr.tr-total').before(htmlCenter);
        $('table.tb-sc-right tr.tr-total').before(htmlRight);

        PMS.utility.imeControl($(".payment-amount"), 'disable');
    }

    if (data.overheadCosts.length > 0) {
        var htmlCenter = '';
        var htmlRight = '';
        var count = 1;
        var totalOvcAmount = 0;

        for (var i = 0; i < data.overheadCosts.length; i++) {
            var ovcID = $("#COPY_TYPE").val() == Constant.COPY_PRJ_TYPE.ALL_INFORMATION ? '-' + data.overheadCosts[i].detail_no : data.overheadCosts[i].detail_no;
            var targetTime = data.overheadCosts[i].target_time;
            var amount = data.overheadCosts[i].amount;

            if (count == 1) {
                htmlCenter += '<tr id="' + ovcID + '">';
                htmlRight += '<tr id="' + ovcID + '">';
            }

            if (!isCopy) {
                htmlCenter += ' <td class="td-ovc-content right" headers="' + targetTime + '">'
                    + ' <input name="OVERHEAD_COST_DETAIL_LIST[' + i + '].detail_no" class="ovc-id" type="hidden" value="' + ovcID + '">'
                    + ' <input name="OVERHEAD_COST_DETAIL_LIST[' + i + '].target_time" class="ovc-target-time" type="hidden" value="' + targetTime + '">'
                    + ' <input name="OVERHEAD_COST_DETAIL_LIST[' + i + '].amount" class="money ovc-amount right numeric valid w80" maxlength="9" type="text" id="' + ovcID + '"  alt="' + targetTime + '" value="' + amount + '" oldValue="' + amount + '">'
                    + ' 円</td>';
            }
            totalOvcAmount += parseInt(amount);

            if (count == colLength) {
                var classCompare = totalOvcAmount != PMS.utility.convertMoneyToInt($('.ovc-id[value="' + ovcID + '"]').siblings('.ovc-total-amount').val()) && !isCopy ? 'error-compare' : '';

                htmlCenter += ' </tr>';
                htmlRight += '<td class="right">'
                    + ' <label class="font-normal lbl-money ' + classCompare + '" id="' + ovcID + '">' + (isCopy ? '0' : totalOvcAmount) + '</label>'
                    + ' <label>円</label>'
                    + ' </td> </tr>';

                count = 0;
                totalOvcAmount = 0;
            }

            count++;
        }
        $('table.tb-ovc-center tr.tr-total').before(htmlCenter);
        $('table.tb-ovc-right tr.tr-total').before(htmlRight);
    }

    if (data.memberAssignments.length > 0 || data.paymentDetails.length > 0 || data.overheadCosts.length > 0) {
        resetAllTotalByMonth();
        PMS.utility.formatMoney();
        PMS.utility.formatMoneyLabel();
    }

    setSummaryLabels();

    checkChangePlanCost(data.memberAssignments, colLength);
}

function checkChangePlanCost(memberAssignments, colLength) {
    //var isChange = false;
    var salesType = parseInt($('#PROJECT_INFO_status_id option:selected').attr('data-type'));
    if (salesType == 0) {
        var param = {
            projectId: parseInt($('#PROJECT_INFO_project_sys_id').val())
        };

        var listPlanCostHistory = PMS.utility.getDataByAjax('/PMS06001/GetPlanCostHistory', param);

        for (var i = 0; i < listPlanCostHistory.length; i++) {
            var colMonth = listPlanCostHistory[i]['target_year'] + '/' + (listPlanCostHistory[i]['target_month'].toString().length == 1 ? '0' + listPlanCostHistory[i]['target_month'] : listPlanCostHistory[i]['target_month']);
            if (listPlanCostHistory[i]['plan_cost'] !== parseInt($("input.hdnPlanCost[id='" + listPlanCostHistory[i]['user_sys_id'] + "'][alt='" + colMonth + "']").val())) {
                $('#IS_CHANGE_HISTORY').val(true);
                break;
            }
        }
    }
    //return false;
}
// Set sales type selected
function setSalesType() {
    var salesType = $('#PROJECT_INFO_status_id option:selected').attr('data-type');

    $('#PROJECT_INFO_sales_type').val(salesType);
}

// Check valid start date of project
function checkStartDate(startDate) {
    if (startDate.length > 0) {
        var errInvalid = PMS.utility.validDate(startDate, Constant.DATE_FORMAT, Constant.PERIOD_START);

        if (errInvalid != null) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, errInvalid);
            return;
        }

        var endDate = $('#PROJECT_INFO_end_date').val();
        var errInvalidEndDate = PMS.utility.validDate(endDate, Constant.DATE_FORMAT, Constant.PERIOD_END);

        if (errInvalidEndDate == null) {
            if (endDate.length > 0 && !PMS.utility.compareDate(startDate, endDate, Constant.DATE_FORMAT)) {
                PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_COMPARE_DATE);
                return;
            }

            if (endDate.length > 0 && !PMS.utility.validateRangeYear(startDate, endDate, Constant.MAX_MONTH_PROJECT_DURATION)) {
                PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_MAX_MONTH_PROJECT_DURATION);
                return;
            }
        }
        resetDataByStartDate(startDate);

        if ($('#PROJECT_INFO_end_date').val() == '') {
            var startDate = new Date(startDate);
            var tempEndDate = new Date(startDate.setMonth(startDate.getMonth() + 1));
            var endDate = new Date(tempEndDate.setDate(tempEndDate.getDate() - 1));
            var endMonth = endDate.getMonth() + 1;
            var endDay = endDate.getDate();

            var strEndDate = endDate.getFullYear() + '/' + (endMonth.toString().length == 1 ? '0' + endMonth : endMonth) + '/' + (endDay.toString().length == 1 ? '0' + endDay : endDay);

            $('#PROJECT_INFO_end_date').val(strEndDate);
            $('.date.datepicker-days.end-date').datepicker("update", strEndDate);
            $('#PROJECT_INFO_end_date').change();
        }
    }
}

// Check valid end date of project
function checkEndDate(endDate) {
    if (endDate.length > 0) {
        var errInvalid = PMS.utility.validDate(endDate, Constant.DATE_FORMAT, Constant.PERIOD_END);

        if (errInvalid != null) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, errInvalid);
            return;
        }

        var startDate = $('#PROJECT_INFO_start_date').val();
        var errInvalidStartDate = PMS.utility.validDate(startDate, Constant.DATE_FORMAT, Constant.PERIOD_START);

        if (errInvalidStartDate != null) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, errInvalidStartDate);
            return;
        }

        if (startDate.length > 0 && !PMS.utility.compareDate(startDate, endDate, Constant.DATE_FORMAT)) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_COMPARE_DATE);
            return;
        }

        if (startDate.length > 0 && !PMS.utility.validateRangeYear(startDate, endDate, Constant.MAX_MONTH_PROJECT_DURATION)) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_MAX_MONTH_PROJECT_DURATION);
            return;
        }

        var shortEndDate = endDate.split('/')[0] + '/' + endDate.split('/')[1];
        var shortOldEndDate = $('#hdnTempTo').val().split('/')[0] + '/' + $('#hdnTempTo').val().split('/')[1];

        if (shortEndDate === shortOldEndDate)
            return;

        if (PMS.utility.compareDate(shortEndDate, shortOldEndDate, Constant.SHORT_DATE_FORMAT)
            && $('table.tb-detail tr.tb-header th.th-month').length > 0
            && $('table.tb-detail tr:not(.tb-header, .tr-total)').length > 0
            && startDate.length > 0) {

            var projectId = $('#PROJECT_INFO_project_sys_id').val();

            if (projectId !== '0') {
                if (checkExistActualWorkTimeByMonthYear(projectId)) {
                    PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_CHANGE_DURATION_TIME);
                    $('#PROJECT_INFO_end_date').val($('#hdnTempTo').val());
                    return;
                }
            }

            BootstrapDialog.show({
                title: '<i class="fa fa-warning"></i>',
                message: Constant.CONFIRM_CHANGE_COLUM,
                closable: false,
                buttons: [{
                    label: 'OK',
                    hotkey: 13,
                    cssClass: 'btn dark',
                    action: function (dialog) {
                        $('#hdnTempTo').val(endDate);
                        $('#hdnOldDuration').attr('changed', 'true');
                        onChangeTime(Constant.PERIOD_START);
                        dialog.close();
                    }
                }, {
                    label: 'キャンセル',
                    cssClass: 'btn light',
                    action: function (dialog) {
                        $('#PROJECT_INFO_end_date').val($('#hdnTempTo').val());
                        dialog.close();
                    }
                }]
            });
        } else {
            $('#hdnTempTo').val(endDate);
            $('#hdnOldDuration').attr('changed', 'true');
            onChangeTime(Constant.PERIOD_START);
        }
    }
}

function expression(month) {
    return
}
// Get unit cost of month
function getUnitCostOfMonth(listUnitCost, month) {
    if (!listUnitCost || !month) {
        return 0;
    }

    var unitCostItem = 0;
    listUnitCost.reverse();
    for (var i = 0; i < listUnitCost.length; i++) {
        if (listUnitCost[i][0] <= month) {
            unitCostItem = listUnitCost[i][1] ? listUnitCost[i][1] : 0;
            break;
        }
    }

    listUnitCost.reverse();

    return unitCostItem;

}

// Event change start date/end date by choose date on calendar
$(".date.datepicker-days").datepicker({
    autoclose: true,
    language: 'ja'
}).on('changeDate', function (ev) {
    var newVal = $(this).children('input').val();

    if ($(this).children('input').hasClass('start_date')) {
        $('#PROJECT_INFO_start_date').val(newVal);
        checkStartDate(newVal);
    }

    if ($(this).children('input').hasClass('end_date')) {
        $('#PROJECT_INFO_end_date').val(newVal);
        checkEndDate(newVal);
    }

    if ($(this).children('input').hasClass('acceptance_date')) {
        $('#PROJECT_INFO_acceptance_date').val(newVal);
    }
});

// Event change value start date
$(document).off('#PROJECT_INFO_start_date');
$(document).on('change', '#PROJECT_INFO_start_date', function () {
    checkStartDate($(this).val());
});

// Event change value end date
$(document).off('#PROJECT_INFO_end_date');
$(document).on('change', '#PROJECT_INFO_end_date', function () {
    checkEndDate($(this).val());
});

// Event get phase list by contract type
$(document).off('#PROJECT_INFO_contract_type_id');
$(document).on('change', '#PROJECT_INFO_contract_type_id', function () {
    var projectId = $('#PROJECT_INFO_project_sys_id').val();
    var isExist = false;

    if (projectId != '0') { // edit case
        $('input.target-phase').each(function () { // check all phase
            var dataSend = {
                projectId: projectId,
                phaseId: $(this).attr('alt')
            }

            if (checkExistActualWorkTimeByPhase(dataSend)) { // exist actual work time by phase
                isExist = true;
                PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_CHANGE_CONTRACT_TYPE);
                return false;
            }
        });
    }

    if (isExist) { // exist actual work time by phase
        $('#PROJECT_INFO_contract_type_id').val($('#hdnOldContractType').val()); // set value after by old value
        return;
    }

    var salesPicFlg = $(this).children('option:selected').attr('data-type') == '0' ? false : true;
    var $salesPicContent = $('.sales-pic-content');

    if (!salesPicFlg)
        $salesPicContent.hide().children('input').val('').attr('title', '');

    if (salesPicFlg)
        $salesPicContent.show();

    var param = {
        contractTypeId: $(this).val(),
        projectID: parseInt(projectId),
        needCheck: ($(this).val() == $('#hdnOldContractType').val())
    };
    var data = PMS.utility.getDataByAjax('/PMS06001/PhaseListJson', param);
    var html = '';

    $('#phase-list').empty(); //remove old data
    html += '<tbody><tr class="tb-header">';
    html += '<th class="th-select-phase">';
    html += '<input type="checkbox" id="check-all-phase"></th>';
    html += '<th class="th-phase-name">フェーズ</th>';
    html += '<th class="th-est-effort">予定工数</th></tr>';
    for (var i = 0; i < data.length; i++) {
        var phaseId = 'PHASE_LIST[' + i + '].phase_id';
        var check = 'PHASE_LIST[' + i + '].check';
        var estimate_man_days = 'PHASE_LIST[' + i + '].estimate_man_days';
        var count = i + 1;
        html += '<tr>';
        html += '<td><input type="checkbox" alt="' + data[i].phase_id + '" class="target-phase" name="' + check + '"value="false">';
        html += '<input type="hidden" name="' + phaseId + '" value="' + data[i].phase_id + '"</td>';
        html += '<input type="hidden" name="' + check + '" value="false"</td>';
        html += '<td><div class="text-overflow" title="' + PMS.utility.htmlEncode(data[i].display_name) + '">' + PMS.utility.htmlEncode(data[i].display_name) + '</div></td>';
        if (data[i].estimate_target_flg === "1") {
            html += '<td><input type="text" class="right decimal est-effort" maxlength="6" name="' + estimate_man_days + '" oldValue="0" disabled="disabled"> 人日</td>';
        }
        else {
            html += '<td></td>';
        }
        html += '</tr>';
    }
    html += '<tr class="tr-total-phase">';
    html += '<td class="empty-td"> </td>';
    html += '<td>合計</td>';
    html += '<td class="total-effort right">0.0 人日</td>';
    html += '</tr>';
    html += '</tbody>';
    $('#phase-list').append(html);
    $('#check-all-phase').change(function () {
        if (this.checked) {
            $('.target-phase').prop('checked', true);
            $('.target-phase').val("true");
            $('.target-phase').each(function () {
                var tbEffort = $(this).parents('tr').find('.est-effort');
                if (tbEffort.attr("disabled") == "disabled") {
                    tbEffort.val(tbEffort.attr('oldValue'));
                    tbEffort.prop('disabled', '');
                }
            });
        } else {
            $('.target-phase').prop('checked', false);
            $('.target-phase').val("false");
            $('.target-phase').each(function () {
                var tbEffort = $(this).parents('tr').find('.est-effort');
                tbEffort.attr('oldValue', tbEffort.val());
                tbEffort.val('');
                tbEffort.prop('disabled', 'disabled');
            });
        }
        resetAllTotalPhaseEffort();
    });


    var $categoryLast = $('div.category-content').last();
    $('.category-list').empty();

    var cateParam = {
        contractTypeId: $(this).val(),
        projectID: parseInt(projectId),
    };
    var cateData = PMS.utility.getDataByAjax('/PMS06001/CategoryListJson', cateParam);
    if (cateData.length > 0) {
        html = '<span class="RequiredField">*</span>'
        $('.category_label').children('span').remove();
        $('.category_label').append(html)

        for (var i = 0; i < cateData.length; i++) {
            var cateId = cateData[i];
            $categoryLast.children('.ddlCategory, .btnDeleteCategory').removeClass("hidden").addClass("hidden");
            $categoryLast.children('.ddlCategory').val(cateId);
            $categoryLast.children('#hdnCategory').val(cateId);
            $categoryLast.children('#hdnSubCategory').val('0');
            $categoryLast.children('label').first().removeClass('hidden').text($categoryLast.find('.ddlCategory option:selected').text());

            $('.category-list').append($categoryLast.prop('outerHTML'));
        }
        setArrCategory();
    }
    else {
        $('.category_label').children('span').remove();
        $categoryLast.children('label').first().removeClass('hidden').addClass("hidden");
        $categoryLast.children('.ddlCategory, .btnDeleteCategory').removeClass("hidden");
        $categoryLast.children('.ddlCategory').val('');
        $categoryLast.children('.ddlSubCategory').empty();
        $categoryLast.children('#hdnCategory, #hdnSubCategory').val('0');
        $('.category-list').append($categoryLast.prop('outerHTML'));
    }
    $('.category-list').animate({ scrollTop: 0 }, 0);

    resetArrCategory();

    $('#hdnUserChangeValue').val(true);
});

// Event select/unselect all target phase
$('#check-all-phase').change(function () {
    if (this.checked) {
        $('.target-phase').prop('checked', true);
        $('.target-phase').val("true");
        $('.target-phase').each(function () {
            var tbEffort = $(this).parents('tr').find('.est-effort');
            if (tbEffort.attr("disabled") == "disabled") {
                tbEffort.val(tbEffort.attr('oldValue'));
                tbEffort.prop('disabled', '');
            }
        });
    } else {
        $('.target-phase').prop('checked', false);
        $('.target-phase').val("false");
        $('.target-phase').each(function () {
            var tbEffort = $(this).parents('tr').find('.est-effort');
            tbEffort.attr('oldValue', tbEffort.val());
            tbEffort.val('');
            tbEffort.prop('disabled', 'disabled');
        });
    }
    resetAllTotalPhaseEffort();
});

// Event get sub category list by category
$(document).off('select.ddlCategory');
$(document).on('change', 'select.ddlCategory', function () {
    var categoryId = $(this).val();
    var $parent = $(this).parent();

    if (categoryId.length == 0) {
        $parent.children('select.ddlSubCategory').empty();
        return;
    }

    var subCategoryList = getSubCategoryList(categoryId);

    $parent.children('select.ddlSubCategory').empty();

    if (subCategoryList.length > 0) {
        $parent.children('select.ddlSubCategory').append('<option value="">' + Constant.NONE_SPECIFIED + '</option>');

        $.each(subCategoryList, function (i, item) {
            $parent.children('select.ddlSubCategory').append('<option value="' + item.sub_category_id + '">' + PMS.utility.htmlEncode(item.sub_category) + '</option>');
        });
    }
});

// Event select sub category
$(document).off('select.ddlSubCategory');
$(document).on('change', 'select.ddlSubCategory', function () {
    var subCategoryId = $(this).val();
    var exist = false;

    $(this).addClass('selected');

    $('select.ddlSubCategory:not(.selected)').each(function () {
        if (subCategoryId == $(this).val() && typeof ($(this).val()) == 'string' && $(this).val().length > 0) {
            exist = true;
            return false;
        }
    });
    $(this).removeClass('selected');

    if (exist) {
        PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.SUBCATEGORY_HAS_BEEN_SELECTED);
        $(this).val('');
        return;
    }
});

// Action add a target category
$(document).off('button.btnAddCategory');
$(document).on('click', 'button.btnAddCategory', function () {
    $('div.category-content').last().after($('div.category-content').first().prop('outerHTML'));

    var $categoryLast = $('div.category-content').last();
    $categoryLast.children('label').first().removeClass('hidden').addClass("hidden");
    $categoryLast.children('.ddlCategory, .btnDeleteCategory').removeClass("hidden");
    $categoryLast.children('.ddlCategory').val('');
    $categoryLast.children('.ddlSubCategory').empty();
    $categoryLast.children('#hdnCategory, #hdnSubCategory').val('0');

    $('.category-list').animate({ scrollTop: $('.category-list')[0].scrollHeight }, 400);

    resetArrCategory();

    $('#hdnUserChangeValue').val(true);
});

// clear selected value
$(document).off('.lblClearValue');
$(document).on('click', '.lblClearValue', function () {
    $(this).parents('.selected-value').children('input').val('').attr('title', '').attr('value', '');
    $('#hdnUserChangeValue').val(true);
});

// Action delete a target category
$(document).off('.btnDeleteCategory');
$(document).on('click', '.btnDeleteCategory', function () {
    $categoryDel = $(this).parent();

    if ($('div.category-content').length > 1) {
        $categoryDel.remove();

        resetArrCategory();
    } else {
        $categoryDel.children('.ddlCategory').val('');
        $categoryDel.children('.ddlSubCategory').empty();
        $categoryDel.children('#hdnCategory, #hdnSubCategory').val('0');
    }

    $('#hdnUserChangeValue').val(true);
});

// Event set summary label
$(document).off('#PROJECT_INFO_tax_rate');
$(document).on('change', '#PROJECT_INFO_tax_rate', function () {
    setSummaryLabels();
    //update data in summary
    var totalIndividualSales = $('#hdnTotalOrder').val();
    var tax = $('#PROJECT_INFO_tax_rate').val();
    var taxRate = PMS.utility.validPositiveNumeric(tax) ? parseInt(tax) / 100 + 1 : 1;
    var roundType = $('#hdnDecimalCalType').val();
    var totalIndividualSalesIncTax = PMS.utility.convertIntToMoney(PMS.utility.roundingDecimal(totalIndividualSales * taxRate, roundType));
    $('#lblTotalOrderIncTax').text(PMS.utility.convertIntToMoney(totalIndividualSalesIncTax) + '円');
});

// Event auto calculate outsourcer sales amount
$(document).off('#PROJECT_INFO_total_sales');
$(document).on('change', '#PROJECT_INFO_total_sales', function () {
    var $osSpan = $('span.out-sourcer');
    var intTotalSales = PMS.utility.convertMoneyToInt($(this).val());
    var strTotalSales = PMS.utility.convertIntToMoney(intTotalSales);

    setSummaryLabels();
});

// Overheadcost item HTML
var ovcHtml = ' <div class="overheadcost-content form-group col-sm-12">'
    + ' <input class="ovc-id" type="hidden" value="">'
    + ' <input class="ovc-pic-id" type="hidden" value="" title=""> '
    + $('.ovc-type-id').first().prop('outerHTML')
    + ' <input class="ovc-detail" maxlength="100" placeholder="購入品目" title="" type="text" value="" readonly="readonly">'
    + ' <input class="ovc-pic-name value-selected" placeholder="購入担当者" readonly="readonly" title="" type="text" value="">'
    + ' <button type="button" class="btn light btnSearchOvcPic disabled" disabled="disabled"><i class="btn-icon btn-search-dialog"></i></button>'
    + ' <input class="ovc-total-amount money right numeric" maxlength="9" size="10" type="text" value="" title="" readonly="readonly">'
    + ' <label class="unit">円</label>'
    + ' <label class="btnDeleteOverheadCost lbl-action">削除</label>'
    + ' </div>';

// Action add more overhead cost
$('#btnAddOverheadCost').click(function () {
    $('div.overheadcost-content').last().after(ovcHtml);

    var $newOverheadCost = $('div.overheadcost-content').last();
    $newOverheadCost.children('.ovc-type-id').val('');

    if ($newOverheadCost.children('.ovc-type-id').val().length > 0)
        $newOverheadCost.children('.ovc-type-id').prepend("<option value='' selected='selected'>指定なし</option>");

    $newOverheadCost.children('.ovc-type-id').val('');
});

// Action delete a outsourcer
$(document).off('.btnDeleteOutsourcer');
$(document).on('click', '.btnDeleteOutsourcer', function () {
    var $osContent = $(this).parents('.out-sourcer-content');

    if ($osContent.find('.old-os-id').val().length > 0)
        $('#hdnUserChangeValue, #IS_CHANGE_HISTORY').val(true);

    $osContent.find('.os-id, .os-name').val('').attr('value', '');
    $('.ddlTagLink ').empty();
    $('.ddlTagLink ').append('<option value="">指定なし</option>');
});

// Action delete a end user
$(document).off('.btnDeleteEndUser');
$(document).on('click', '.btnDeleteEndUser', function () {
    var $euContent = $(this).parents('.end-user-content');

    if ($euContent.find('.old-eu-id').val().length > 0)
        $('#hdnUserChangeValue, #IS_CHANGE_HISTORY').val(true);

    $euContent.find('.eu-id, .eu-name').val('').attr('value', '');
});


// Action delete a subcontractor
$(document).off('.btnDeleteSubcontractor');
$(document).on('click', '.btnDeleteSubcontractor', function () {
    $subcontractorDel = $(this).parent();
    var deleteId = $subcontractorDel.children('.sc-id').val();

    if ($('div.subcontractor-content').length > 1)
        $subcontractorDel.remove();
    else {
        $subcontractorDel.removeClass('has-payment').children('input').val('').attr('value', '').attr('title', '');
        $subcontractorDel.children('.btnSearchSubcontractorPic').attr('disabled', 'disabled');
        $subcontractorDel.children('.sc-payment').attr('readonly', 'readonly');
    }

    if (deleteId.length > 0) {
        $('#hdnUserChangeValue').val(true);

        if (!$('table.tb-sc-left tr[id="' + deleteId + '"]').hasClass('new-data'))
            $('#IS_CHANGE_HISTORY').val(true);


        $('table.tb-sc-left tr[id="' + deleteId + '"], table.tb-sc-center tr[id="' + deleteId + '"], table.tb-sc-right tr[id="' + deleteId + '"]').remove();

        calTotalPayment();
        setSummaryLabels();
        resetArrSubcontractor();
        resetArrPaymentDetail();
        resetAllTotalByMonth();
    }
});

// Action delete a overhead cost
$(document).off('.btnDeleteOverheadCost');
$(document).on('click', '.btnDeleteOverheadCost', function () {
    var $overheadCostDel = $(this).parent();
    var deleteId = $overheadCostDel.children('.ovc-id').val();

    if (deleteId.length > 0) {
        $overheadCostDel.removeClass('has-payment');
        $('table.tb-ovc-left tr[id="' + deleteId + '"], table.tb-ovc-center tr[id="' + deleteId + '"], table.tb-ovc-right tr[id="' + deleteId + '"]').remove();

        if ($overheadCostDel.hasClass('old-data')) {
            $('#hdnUserChangeValue, #IS_CHANGE_HISTORY').val(true);
            $overheadCostDel.children('.ovc-delete').val(true);
            $overheadCostDel.addClass('delete').hide();
        } else {
            $overheadCostDel.remove();
        }

        resetArrOverheadCostDetail();
        resetAllTotalByMonth();
        calTotalPayment();
        setSummaryLabels();
    } else {
        $overheadCostDel.remove();
    }

    if ($('div.overheadcost-content:not(".delete")').length == 0) {
        $('div.add-overhead-cost').after(ovcHtml);

        var $ddlOvcType = $('div.overheadcost-content:not(".delete")').last().children('.ovc-type-id');

        if ($ddlOvcType.html().indexOf('指定なし') == -1) {
            $('div.overheadcost-content:not(".delete")').last().children('.ovc-type-id').prepend("<option value='' selected='selected'>指定なし</option>");
        }

        $ddlOvcType.val('');
    }

    resetArrOverheadCost();
});

// Event auto calculate total payment when change value amount of supplier
$(document).off('.sc-payment');
$(document).on('change', '.sc-payment', function () {
    var scID = $(this).siblings('.sc-id').val();
    var $totalPaymentAmount = $('table.tb-sc-right label[id="' + scID + '"]');

    if (PMS.utility.convertMoneyToInt($(this).val()) != PMS.utility.convertMoneyToInt($totalPaymentAmount.text()))
        $totalPaymentAmount.addClass('error-compare');
    else
        $totalPaymentAmount.removeClass('error-compare');

    calTotalPayment();
    setSummaryLabels();
});

// Event change of overhead cost type
$(document).off('select.ovc-type-id');
$(document).on('change', 'select.ovc-type-id', function () {
    var $parent = $(this).parent();
    var $ovcId = $parent.children('.ovc-id');
    var ovcIdVal = $ovcId.val();
    var typeName = PMS.utility.htmlDecode($(this).children('option:selected').html());
    var ovcDetailName = $(this).siblings('.ovc-detail').val().length > 0 ? ' : ' + $(this).siblings('.ovc-detail').val() : '';

    if ($(this).val().length > 0) {
        $parent.addClass('has-payment').children('.btnSearchOvcPic').removeClass('disabled').removeAttr('disabled');
        $parent.children('.ovc-detail').removeAttr('readonly');

        if (ovcIdVal.length > 0) {
            // replace old value by new selected
            $('table.tb-ovc-left tr[id="' + ovcIdVal + '"]').find('td.td-ovc-type div.text-overflow').attr('title', typeName + ovcDetailName).text(typeName + ovcDetailName);
            $parent.children('.ovc-change').val(true);

            if (parseInt(ovcIdVal) > 0)
                $('#hdnUserChangeValue').val(true);
        } else {
            // add new
            var newOvcId = $parent.index() * -1;
            $ovcId.val(newOvcId);

            var colums = PMS.utility.getMonthCols($('#PROJECT_INFO_start_date').val(), $('#PROJECT_INFO_end_date').val());
            var htmlLeft = '<tr id="' + newOvcId + '" class="new-data">'
                + " <td class='td-ovc-type'> <div class='text-overflow' title='" + typeName + "'>" + PMS.utility.htmlEncode($(this).children('option:selected').text()) + "</div></td>"
                + '</tr>';
            var htmlRight = '<tr id="' + newOvcId + '"> <td class="right"> <label class="font-normal lbl-money" id="' + newOvcId + '">0</label> <label> 円</label></td></tr>';
            var htmlCenter = '<tr id="' + newOvcId + '">';

            for (var j = 0; j < colums.length; j++) {
                htmlCenter += ' <td class="td-ovc-content right" headers="' + colums[j] + '">'
                    + ' <input class="ovc-id" type="hidden" value="' + newOvcId + '">'
                    + ' <input class="ovc-target-time" type="hidden" value="' + colums[j] + '">'
                    + ' <input class="money ovc-amount right numeric valid w80" maxlength="9" type="text" id="' + newOvcId + '"  alt="' + colums[j] + '">'
                    + ' <label> 円</label></td>';
            }
            htmlCenter += '</tr>';

            $('table.tb-ovc-left tr.tr-total').before(htmlLeft);
            $('table.tb-ovc-center tr.tr-total').before(htmlCenter);
            $('table.tb-ovc-right tr.tr-total').before(htmlRight);
        }
    } else {
        // delete new overhead cost
        $parent.removeClass('has-payment').children('.ovc-id, .ovc-pic-id, .ovc-detail, .ovc-pic-name, .ovc-total-amount').val('');
        $parent.children('.btnSearchOvcPic').addClass('disabled').attr('disabled', 'disabled');
        $parent.children('.ovc-detail, .ovc-total-amount').attr('readonly', 'readonly');
        $('table.tb-ovc-left tr[id="' + ovcIdVal + '"], table.tb-ovc-center tr[id="' + ovcIdVal + '"], table.tb-ovc-right tr[id="' + ovcIdVal + '"]').remove();
    }

    displaySubcontractor();
    resetArrOverheadCost();
    resetArrOverheadCostDetail();
});

// Event auto calculate total payment when change value amount of overhead cost
$(document).off('.ovc-total-amount');
$(document).on('change', '.ovc-total-amount', function () {
    var ovcID = $(this).siblings('.ovc-id').val();
    var $totalOvcAmount = $('table.tb-ovc-right label[id="' + ovcID + '"]');

    if (PMS.utility.convertMoneyToInt($(this).val()) != PMS.utility.convertMoneyToInt($totalOvcAmount.text()))
        $totalOvcAmount.addClass('error-compare');
    else
        $totalOvcAmount.removeClass('error-compare');

    $(this).parent().children('.ovc-change').val(true);
    calTotalPayment();
    setSummaryLabels();
});

// Event auto calculate total individual sales of a member
$(document).off('input.ma-sales');
$(document).on('change', 'input.ma-sales', function () {
    resetTotalIndividualSales($(this).attr('id'));
    resetAllTotalByMonth();
});

// Event auto set summary label when unit price of member has changed
$(document).off('input.unit-cost');
$(document).on('change', 'input.unit-cost', function () {
    setSummaryLabels();
});


// Event change plan man days
$(document).off('input.ma-man-days');
$(document).on('change', 'input.ma-man-days', function () {
    var roundType = $('#hdnDecimalCalType').val();
    var thisMonth = $(this).attr("alt");
    var maID = $(this).attr('id');
    var workDay = getWorkDayOfMonth(thisMonth, thisMonth) ? getWorkDayOfMonth(thisMonth, thisMonth)[0] : 0;
    var planManDay = $(this).val();
    var unitPrice = $('table.tb-ma-center tr[id="' + maID + '"] td.td-ma-detail[headers="' + thisMonth + '"] input.ma-unit-cost-in-month').val();
    var planCost = workDay == 0 ? 0 : PMS.utility.roundingDecimal(unitPrice * planManDay / workDay, roundType);

    $('table.tb-ma-sales-center td.td-ma-sales-detail[headers="' + thisMonth + '"] span.plan-cost[id="' + maID + '"]').text(PMS.utility.convertIntToMoney(planCost));
    $('table.tb-ma-sales-center td.td-ma-sales-detail[headers="' + thisMonth + '"] input.hdnPlanCost[id="' + maID + '"]').val(planCost);
    resetTotalIndividualSales(maID);
});

// Event click delete member assignment
$(document).off('.btnDeleteMa');
$(document).on('click', '.btnDeleteMa', function () {
    var projectId = $('#PROJECT_INFO_project_sys_id').val();
    var maId = $(this).parent().attr('data-id');

    if (maId == $('#PROJECT_INFO_charge_person_id').val()) {
        PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_DELETE_PIC);
        return;
    }

    if (projectId == '0') {
        deleteMemberAssignment(maId);
        resetAllTotalByMonth();
        return;
    }

    var param = {
        projectId: projectId,
        userId: maId
    }
    var data = PMS.utility.getDataByAjax('/PMS06001/CheckDeleteMember', param);

    if (data.actual_work_time > 0
        || data.work_start_time > 0
        || data.rest_time > 0
        || data.work_end_time > 0) {
        PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_DELETE_MEMBER);
        return;
    }

    $('#hdnUserChangeValue').val(true);
    deleteMemberAssignment(maId);
    resetAllTotalByMonth();
});

// Do auto allocation payment
$(document).off('#autoAllocationPayment');
$(document).on('click', '#autoAllocationPayment', function () {
    $('.sc-id').each(function () {
        var scID = $(this).val();
        var payment = PMS.utility.convertMoneyToInt($(this).siblings('.sc-payment').val());
        var $target = $('.payment-amount[id="' + scID + '"]');
        var targetQuantity = $target.length;
        var amount = parseInt(payment / targetQuantity);
        var overbalance = payment - (amount * targetQuantity); // get overbalance

        $target.val(amount);

        if (overbalance > 0) {
            $target.last().val(amount + overbalance);
        }

        $('table.tb-sc-right tr:not(.tb-header, .tr-total) label[id="' + scID + '"]').text(PMS.utility.convertIntToMoney(payment)).removeClass('error-compare');
    });

    PMS.utility.formatMoney('input.payment-amount');
    resetTotalPaymentByMonth();
});

// Do auto allocation payment
$(document).off('#autoAllocationOverheadCost');
$(document).on('click', '#autoAllocationOverheadCost', function () {
    $('.overheadcost-content .ovc-id').each(function () {
        var ovcID = $(this).val();
        var overheadCost = PMS.utility.convertMoneyToInt($(this).siblings('.ovc-total-amount').val());
        var $target = $('.ovc-amount[id="' + ovcID + '"]');
        var targetQuantity = $target.length;
        var amount = parseInt(overheadCost / targetQuantity);
        var overbalance = overheadCost - (amount * targetQuantity); // get overbalance

        $target.val(amount);

        if (overbalance > 0) {
            $target.last().val(amount + overbalance);
        }

        $('table.tb-ovc-right tr:not(.tb-header, .tr-total) label[id="' + ovcID + '"]').text(PMS.utility.convertIntToMoney(overheadCost)).removeClass('error-compare');
    });

    PMS.utility.formatMoney('input.ovc-amount');
    resetTotalOverheadCostByMonth();
});

// Event auto calculate total payment amount of a subcontractor
$(document).off('input.payment-amount');
$(document).on('change', 'input.payment-amount', function () {
    resetTotalPaymentDetail($(this).attr('id'));
    resetTotalPaymentByMonth();
});

// Event auto calculate total payment amount of a subcontractor
$(document).off('input.ovc-amount');
$(document).on('change', 'input.ovc-amount', function () {
    resetTotalOvcDetail($(this).attr('id'));
    resetTotalOverheadCostByMonth();
});

// Event uncheck target phase
$(document).off('input.target-phase');
$(document).on('change', 'input.target-phase', function () {
    var projectId = $('#PROJECT_INFO_project_sys_id').val();
    var $checkbox = $(this);
    $checkbox.val($checkbox.prop('checked'));
    if (projectId != '0' && $checkbox.prop('checked') == false) {
        var dataSend = {
            projectId: projectId,
            phaseId: $checkbox.attr('alt')
        }

        if (checkExistActualWorkTimeByPhase(dataSend)) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_DELETE_PHASE);
            $checkbox.prop('checked', true);
            return;
        }
    }
});

// Event add a progress
$(document).on('click', 'button.btnAddProgress', function () {
    var $contentElement = $(this).parent()
    var newRegistDate = $contentElement.find('.progress-regist-date').val();
    var errRegistDate = PMS.utility.validDate(newRegistDate, Constant.DATE_FORMAT, Constant.PROGRESS_REGIST_DATE);

    if (newRegistDate.length == 0) {
        PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.PROGRESS_REGIST_DATE + Constant.ERR_REQUIRED);
        return;
    }

    if (errRegistDate != null) {
        PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, errRegistDate);
        return;
    }

    var isExist = false;
    var isUpd = false;
    $('.hdnProgressRegistDate').each(function (index, element) {
        if ($(element).val() == newRegistDate) {
            if ($(element).closest('tr').hasClass('deleted')) {
                isUpd = true;
                return;
            }
            else {
                isExist = true;
                return false;
            }
            
        }
    })

    if (isExist) {
        PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_DUPLICATE_PROGRESS_REGIST_DATE);
        return;
    }

    var newProgress = $contentElement.find('.progress-percent').val();
    var newRemarks = $contentElement.find('.progress-remarks').val();

    if (newProgress.length == 0) {
        PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.PROGRESS + Constant.ERR_REQUIRED);
        return;
    } else {
        if (!PMS.utility.validPositiveNumeric(newProgress)) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.PROGRESS + Constant.ERR_FORMAT);
            return;
        }
        else if (parseInt(newProgress) > 100) {
            PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_RANGE.replace('{0}', Constant.PROGRESS).replace('{1}', Constant.MAX_PERCENT));
            return;
        }
    }

    if (newRemarks.length > Constant.MAX_AREA_TEXT) {
        PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, Constant.ERR_STRING_LENGTH.replace('{0}', '備考').replace('{1}', Constant.MAX_AREA_TEXT));
        return;
    }

    var registDate = $.datepicker.formatDate('yy/mm/dd', new Date(newRegistDate));
    var html = '<tr class="new-data">'
        + ' <td>'
        + registDate
        + ' <input class="hdnProgressRegistDate" type="hidden" value="' + registDate + '">'
        + ' </td>'
        + ' <td>'
        + ' <input class="hdnProgressPercent" type="hidden" value="' + newProgress + '">'
        + ' ' + newProgress + '%'
        + ' </td>'
        + ' <td>'
        + " <input class='hdnProgressRemarks' type='hidden'>"
        + " <div class='text-overflow short-progress-remarks'></div>"
        + ' </td>'
        + ' <td class="td-remove">'
        + ' <input class="hdnIsNew" type="hidden" value="True"><label class="lbl-action btnDeleteProgress">削除</label></td>'
        + ' </tr>';
    if (isUpd) {
        scrollToNewDataPosition();

        $contentElement.find('input').val('').attr('value', '');
        return;

    }
    if ($('.progress-list .tb-detail tr').length == 0)
        $('.progress-list .tb-detail tbody').append(html);
    else {
        var isSmallest = true;

        $('.hdnProgressRegistDate').each(function () {
            if (!PMS.utility.compareDate(registDate, $(this).val(), Constant.DATE_FORMAT)) {
                $(this).parents('tr').before(html);
                isSmallest = false;
                return false;
            }
        });

        if (isSmallest) {
            $('.progress-list .tb-detail tr').last().after(html);
        }
    }

    var $target = $('.hdnProgressRegistDate[value="' + registDate + '"]').parents('tr');
    $target.find('.hdnProgressRemarks').val(newRemarks);
    $target.find('.short-progress-remarks').text(newRemarks).attr('title', newRemarks);

    $('.progress-latest label').first().text($('.hdnProgressRegistDate').first().val());
    $('.progress-latest label').last().text($('.hdnProgressPercent').first().val() + '%');
    $('#lblProgressInfo').html($('.hdnProgressPercent').first().val() + '%');
    scrollToNewDataPosition();
    resetArrProgress();

    $contentElement.find('input').val('').attr('value', '');
    $('#hdnUserChangeValue').val(true);
    function scrollToNewDataPosition() {
        progressContentArr = $('.progress-list .tb-detail tr');
        for (var i = 0; i < progressContentArr.length; i++) {
            if ($(progressContentArr[i]).find('.hdnProgressRegistDate').val() == registDate) {
                var ypos = $('.progress-list .tb-detail tr:eq(' + i + ')').offset().top;
                $('.progress-list').animate({ scrollTop: ypos - $('.progress-list .tb-detail').offset().top });
            }
        }
    }

});

// Event delete a progress
$(document).off('.btnDeleteProgress');
$(document).on('click', '.btnDeleteProgress', function () {
    var $parent = $(this).parents('tr');
    if ($parent.hasClass('new-data')) {
        $parent.remove();
    } else {
        $(this).siblings('.hdnIsDelete').val(true);
        $parent.addClass('deleted').hide();
    }

    var registDateText = '';
    var progressPercentText = '';

    if ($(".progress-list .tb-detail tr:not(.deleted)").length > 0) {
        var $firstElement = $(".progress-list .tb-detail tr:not(.deleted)").first();
        registDateText = $firstElement.find(".hdnProgressRegistDate").val();
        progressPercentText = $firstElement.find(".hdnProgressPercent").val() + '%';
    }

    $('.progress-latest label').first().text(registDateText);
    $('.progress-latest label').last().text(progressPercentText);

    resetArrProgress();
});

// Set status of user change value
function setChangeValueStatus() {
    if (parseInt($('#PROJECT_INFO_status_id').attr('oldvalue')) != 0) {
        $('#IS_CHANGE_HISTORY').val(true);
    } else if ($('tr.new-data').length > 0) {
        $('#IS_CHANGE_HISTORY').val(true);

        if ($('.tb-ma-left tbody tr.new-data').length > 0)
            $('#IS_UPDATE_ASSIGN_DATE').val(true);
    } else {
        var isChanged = false;

        if ($('.os-id').val() != $('.old-os-id').val()) {
            $('#IS_CHANGE_HISTORY').val(true);
            isChanged = true;
        }

        if ($('.eu-id').val() != $('.old-eu-id').val()) {
            $('#IS_CHANGE_HISTORY').val(true);
            isChanged = true;
        }

        $('.ma-man-days').each(function () {
            if (parseFloat(this.value).toFixed(1) != $(this).attr('oldvalue')) {
                $('#IS_CHANGE_HISTORY, #IS_UPDATE_ASSIGN_DATE').val(true);
                isChanged = true;
                return false;
            }
        });

        if (!isChanged) {
            $('.unit-cost, .ma-sales, .payment-amount, .ovc-amount').each(function () {
                if (this.value.replace(/,/g, '') != $(this).attr('oldvalue').replace(/,/g, '')) {
                    $('#IS_CHANGE_HISTORY').val(true);

                    if ($(this).hasClass('unit-cost'))
                        $('#IS_UPDATE_ASSIGN_DATE').val(true);

                    isChanged = true;
                    return false;
                }
            });
        }

        if (!isChanged) {
            $('.sc-id, .sc-pic-id, .ovc-type-id, .ovc-detail, .ovc-pic-id').each(function () {
                if (typeof ($(this).attr('oldvalue')) != 'undefined' && this.value != $(this).attr('oldvalue')) {
                    $('#IS_CHANGE_HISTORY').val(true);
                    isChanged = true;
                    return false;
                }
            });
        }
    }
}

//Callback handler for form submit event
$("#frmProjectEdit").submit(function (e) {
    //Set end user as outsourcer if end user is blank
    var endUserId = $('#OUTSOURCER_end_user_id').val();
    var outsourcerId = $('#OUTSOURCER_customer_id').val();
    var outsourcerName = $('#OUTSOURCER_customer_name').val();
    if (endUserId == 0) {
        $('#OUTSOURCER_end_user_id').val(outsourcerId);
        $('#OUTSOURCER_end_user_name').val(outsourcerName);
    }

    if (parseInt($('#PROJECT_INFO_status_id option:selected').attr('data-type')) == 0)
        setChangeValueStatus();

    var formData = new FormData(this);

    $.ajax({
        url: $(this).attr("action"),
        type: 'POST',
        data: formData,
        mimeType: "multipart/form-data",
        contentType: false,
        cache: false,
        processData: false,
        success: function (response) {
            var data = JSON.parse(response);

            BootstrapDialog.closeAll();

            if (data.statusCode == 201 || data.statusCode == 202) {
                if (data.statusCode == 201) { // update success
                    $(".project-regist-content").css('visibility', 'hidden');
                    PMS.utility.showMessageDialog(Constant.DIALOG_INFORMATION, data.message, null, null, function () {
                        $("#frmEditByPostBack").children('[name="id"]').val(data.projectID);
                        $("#frmEditByPostBack").children('[name="isNotBack"]').val(data.IsNotBack);
                        $("#frmEditByPostBack").submit();
                    });
                }
                else { // duplicate update
                    //var htmlFormReload = '<form id="frmProjectReload" method="POST" action="/PMS06001/Edit">'
                    //+ '<input type="hidden" name="id" value="' + data.projectID + '" /></form>';
                    //$('body').append(htmlFormReload);

                    $("#frmEditByPostBack").children('[name="id"]').val(data.projectID);
                    $("#frmEditByPostBack").children('[name="isNotBack"]').val(data.IsNotBack);

                    $(".project-regist-content").css('visibility', 'hidden');
                    PMS.utility.showSubmitConfirmDialog(data.message, '#frmEditByPostBack', null, function (action) {
                        if (!action)
                            $(".project-regist-content").css('visibility', '');
                    });
                }
            }

            if (data.statusCode == 500) { // Exception
                $(".project-regist-content").css('visibility', 'hidden');
                PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, data.message, '/PMS06001');
            }

            if (typeof (data.ErrorMessages) !== 'undefined') // invalid data
                PMS.utility.showClientError(data.ErrorMessages);
        },
        error: function (error) {
            if (error.status == 419) { //419: Authentication Timeout
                $(".project-regist-content").css('visibility', 'hidden');
                window.location.href = '/PMS01001/Login/timeout';
            }
        }
    });

    e.preventDefault(); // prevent Default action
});

// Event change status
$('#PROJECT_INFO_status_id').on('change', function () {
    $('#lblStatusInfo').html($('#PROJECT_INFO_status_id option:selected').text());
    setSalesType();
});

// Event click show history page
$(document).off('label.display-history');
$(document).on('click', 'label.display-history', function () {
    var param = { projectID: $('#frmProjectEdit').find('#PROJECT_INFO_project_sys_id').val() };
    var historyList = PMS.utility.getDataByAjax('/PMS06001/HistoryListJson', param);
    var salesType = parseInt($('#PROJECT_INFO_status_id option:selected').attr('data-type'));

    if ((historyList.length < 2 && salesType == 0)
        || (historyList.length === 0 && salesType != 0)) {
        PMS.utility.showMessageDialog(Constant.DIALOG_INFORMATION, '履歴情報が存在しません。');
        return;
    }

    if ($('#hdnUserChangeValue').val() === 'true') {
        PMS.utility.showMessageDialog(Constant.DIALOG_WARNING, '編集中の内容があるため表示できません。');
        return;
    }

    var windowFeatures = "directories=no,titlebar=no,toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=1300,height=500";
    var url = "/PMS06001/History";
    var historyPage = window.open(url, "プロジェクト登録履歴", windowFeatures).focus();
});

// Event set change info
$(document).off('input:text, input:checkbox, select');
$(document).on('change', 'input:text, input:checkbox, select, textarea, input:file', function () {
    $('#hdnUserChangeValue').val(true);
});

// Event auto calculate total payment when change value amount of overhead cost
$(document).off('.ovc-detail');
$(document).on('change', '.ovc-detail', function () {
    var $parent = $(this).parent();

    $parent.children('.ovc-change').val(true);

    var $control = $('table.tb-ovc-left tr[id="' + $parent.children('.ovc-id').val() + '"] div');
    var newTitle = $control.text().split(':')[0].trim() + ' : ' + $(this).val();

    $control.attr('title', newTitle).text(newTitle);
});

// Click close project info on right tab
$('.project-summary-title > i').click(function () {
    $('.project-summary').hide();
    $('.project-regist').css('width', '97%');
    $('.project-regist .col-center').css('max-width', '60%');
    $('.project-regist .col-ma-center').css('max-width', '59%');
    $('.project-summary-small').show();
});

// Click show project info on right tab
$('.project-summary-title-small > i').click(function () {
    // Set the effect type
    var effect = 'slide';

    // Set the options for the effect type chosen
    var options = { direction: "right" };

    // Set the duration (default: 400 milliseconds)
    var duration = 100;
    $('.project-summary-small').hide();
    $('.project-summary').toggle(effect, options, duration);
    $('.project-regist').css('width', '');
    $('.project-regist .col-center').removeAttr('style');
});

// Reset total estimated effort of a phase when it has changed
function resetAllTotalPhaseEffort() {
    var totalEstimatedEffort = 0;
    $('input.est-effort').each(function () {
        var planManDay = $(this).val().trim();
        var planManDayArr = planManDay.split('.');
        var validPlanManDay = true;
        if (!PMS.utility.validDecimalNumeric(planManDay)
            || (typeof planManDayArr[1] != 'undefined'
                && (planManDayArr[1].length == 0
                    || planManDayArr[1].length > Constant.DECIMAL_AFTER_POINT_MAX_LENGTH))) {
            validPlanManDay = false;
        } else if (planManDay.length > Constant.ESTIMATE_TIME_MAX_LENGTH || planManDayArr[0].length > 4) {
            validPlanManDay = false;
        }
        if (validPlanManDay) {
            totalEstimatedEffort += parseFloat(planManDay);
        }
    });
    $('.total-effort').text(parseFloat(totalEstimatedEffort).toFixed(1) + " 人日");
}

// Event auto calculate total estimated effort of all phases
$(document).off('input.est-effort');
$(document).on('change', 'input.est-effort', function () {
    resetAllTotalPhaseEffort();
});

function resetCheckAllPhaseStatus() {
    var checkAllPhase = true;
    $('input.target-phase').each(function () {
        if ($(this)[0].checked == false) {
            checkAllPhase = false;
        }
    })
    $('#check-all-phase')[0].checked = checkAllPhase;
}

$(document).off('input.target-phase');
$(document).on('change', 'input.target-phase', function () {
    resetCheckAllPhaseStatus();
    var tbEffort = $(this).parents('tr').find('.est-effort');
    if ($(this)[0].checked === false) {
        tbEffort.attr('oldValue', tbEffort.val());
        tbEffort.val('');
        tbEffort.prop('disabled', 'disabled');
    }
    else {
        tbEffort.val(tbEffort.attr('oldValue'));
        tbEffort.prop('disabled', '');
    }
    resetAllTotalPhaseEffort();
});
