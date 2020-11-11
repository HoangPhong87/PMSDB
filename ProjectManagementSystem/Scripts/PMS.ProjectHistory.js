/*!
 * File: PMS.ProjectHistory.js
 * Company: i-Enter Asia
 * Copyright © 2014 i-Enter Asia. All rights reserved.
 * Project: Project Management System
 * Author: HoangPS
 * Created date: 2014/11/26
 */

if (opener == null)
    $(document).empty();

var defaultHTML = $('#historyContent').html();
var $parentForm = $(opener.document).find('#frmProjectEdit'); // parent page what show history dialog (content)
var $parentSummary = $(opener.document).find('.project-summary'); // parent page what show history dialog (summary info)
var currentProjectInfo = {
    projectID: $parentForm.find('#PROJECT_INFO_project_sys_id').val(),
    salesType: parseInt($parentForm.find('#PROJECT_INFO_sales_type').attr('alt'))
};

$(document).ready(function () {
    $("body").addClass("content-edit-wrapper");
    bindControlSelectHistory(currentProjectInfo.projectID);
    loadProjectHistory();
});

// Compare history value with current value
function compareValue($control, historyVal, label, money, canNegative) {
    var classChange = "";
    canNegative = typeof (canNegative) == "undefined" ? false : true;

    if ($control.length > 0) {
        var value = label == true ? $control.text().replace('円', '').replace('人日', '') : $control.val();
        var currentVal = money == true ? PMS.utility.convertMoneyToInt(value, canNegative) : parseFloat(value);

        if (currentVal > historyVal)
            classChange = "rise";

        if (historyVal > currentVal)
            classChange = "reduce";
    } else {
        classChange = "reduce";
    }

    return classChange;
}

// Bind data for dropdownlist select history
function bindControlSelectHistory(projectID) {
    var param = { projectID: projectID };
    var historyList = PMS.utility.getDataByAjax('/PMS06001/HistoryListJson', param);
    var $dllHistory = $('#dllHistory');

    if (currentProjectInfo.salesType == 0)
        historyList.shift();

    $.each(historyList, function (i, item) {
        $dllHistory.append('<option aria-valuetext="' + item.delete_status + '" value="' + item.history_no + '">' + item.insert_date + '</option>');
    });
}

// Bind data for member assignment list
function bindMemberAssigmentList(targetTimes, memberAssignments, memberAssignmentDetails) {
    var totalIndividualSales = 0;
    var totalPlanCost = 0;

    if (memberAssignments.length > 0) {
        var htmlMaLeft = '';
        var htmlMaSalesLeft = '';
        var htmlMaCenter = '';
        var htmlMaSalesCenter = '';
        var htmlMaRight = '';
        var htmlMaSalesRight = '';

        var colLength = targetTimes.length;

        for (var i = 0; i < memberAssignments.length; i++) {
            var totalIndividualSalesPerMem = 0;
            var totalPlanCostPerMem = 0;
            var classHistory, classUnitCost = "";
            var $current = $parentForm.find('table.tb-ma-left input.ma-id[value="' + memberAssignments[i].user_sys_id + '"]');

            if (currentProjectInfo.salesType == 0) {
                classHistory = $current.length > 0 ? "" : "reduce";

                if ($current.length > 0) {
                    var $currentUnitCost = $parentForm.find('input.unit-cost[alt="' + memberAssignments[i].user_sys_id + '"]');
                    classUnitCost = compareValue($currentUnitCost, memberAssignments[i].unit_cost, false, true);
                } else {
                    classUnitCost = classHistory;
                }
            }

            htmlMaLeft += "<tr alt='" + memberAssignments[i].user_sys_id + "'>"
                + "<td><div class='ma-display-name text-overflow " + classHistory + "' title='" + memberAssignments[i].display_name + "'>" + PMS.utility.htmlEncode(memberAssignments[i].display_name) + "</div></td>"
                //+ "<td class='td-ma right'>"
                //+ "<label class='font-normal money unit-cost " + classUnitCost + "'>" + memberAssignments[i].unit_cost + "</label> "
                //+ "<label>円</label></td>";
                +'</tr>';
            htmlMaSalesLeft += "<tr alt='" + memberAssignments[i].user_sys_id + "'>"
                + "<td><div class='ma-sales-display-name text-overflow " + classHistory + "' title='" + memberAssignments[i].display_name + "'>" + PMS.utility.htmlEncode(memberAssignments[i].display_name) + "</div></td>"
                + "</tr>";

            htmlMaCenter += '<tr alt="' + memberAssignments[i].user_sys_id + '">';
            htmlMaSalesCenter += '<tr alt="' + memberAssignments[i].user_sys_id + '">';

            for (var j = 0; j < colLength; j++) {
                var index = i * colLength + j;
                var classMa, classMaSales, classPlanCost = "";

                if (currentProjectInfo.salesType == 0) {
                    if ($current.length > 0) {
                        var $currentMa = $parentForm.find('input.ma-man-days[alt="' + targetTimes[j] + '"][id="' + memberAssignments[i].user_sys_id + '"]');
                        var $currentMaSales = $parentForm.find('input.ma-sales[alt="' + targetTimes[j] + '"][id="' + memberAssignments[i].user_sys_id + '"]');
                        var $currentPlanCost = $parentForm.find('span.plan-cost[alt="' + targetTimes[j] + '"][id="' + memberAssignments[i].user_sys_id + '"]');
                        classMa = compareValue($currentMa, memberAssignmentDetails[index].plan_man_days, false, false);
                        classMaSales = compareValue($currentMaSales, memberAssignmentDetails[index].individual_sales, false, true);
                        classPlanCost = compareValue($currentPlanCost, memberAssignmentDetails[index].plan_cost, true, true)
                    } else {
                        classMa = classMaSales = classPlanCost = classHistory;
                    }
                }

                totalIndividualSalesPerMem += memberAssignmentDetails[index].individual_sales;
                totalPlanCostPerMem += memberAssignmentDetails[index].plan_cost;

                htmlMaCenter += '<td class="right">'
                    + '<label class="font-normal ma-man-days ' + classMa + '" alt="' + targetTimes[j] + '">'
                    + memberAssignmentDetails[index].plan_man_days.toFixed(1) + '</label> '
                    + '<label>人日</label></td>';
                htmlMaSalesCenter += '<td class="right">'
                    + '<div class="div-sales"><div><label class="font-normal ma-sales money ' + classMaSales + '" alt="' + targetTimes[j] + '">'
                    + memberAssignmentDetails[index].individual_sales + '</label> '
                    + '<label>円 /&nbsp;</label></div><div><label class="font-normal plan-cost ' + classPlanCost + '" alt="' + targetTimes[j] + '">' + PMS.utility.convertIntToMoney(memberAssignmentDetails[index].plan_cost) + '</label><label>&nbsp;円</label></div></div></td>';
            }

            totalIndividualSales += totalIndividualSalesPerMem;
            totalPlanCost += totalPlanCostPerMem;

            htmlMaCenter += '</tr>';
            htmlMaSalesCenter += '</tr>';

            var classMaTotal, classMaSalesTotal, classPlanCostTotal = "";

            if (currentProjectInfo.salesType == 0) {
                if ($current.length > 0) {
                    var $currentMaTotal = $parentForm.find('table.tb-ma-right label[id="' + memberAssignments[i].user_sys_id + '"]');
                    var $currentMaSalesTotal = $parentForm.find('table.tb-ma-sales-right label.ma-sales-by-user[id="' + memberAssignments[i].user_sys_id + '"]');
                    var $currentPlanCostTotal = $parentForm.find('table.tb-ma-sales-right label.plan-cost-by-user[id="' + memberAssignments[i].user_sys_id + '"]');
                    classMaTotal = compareValue($currentMaTotal, memberAssignments[i].total_plan_man_days, true, false);
                    classMaSalesTotal = compareValue($currentMaSalesTotal, totalIndividualSalesPerMem, true, true);
                    classPlanCostTotal = compareValue($currentPlanCostTotal, totalPlanCostPerMem, true, true);
                } else {
                    classMaTotal = classMaSalesTotal = classPlanCostTotal = classHistory;
                }
            }

            htmlMaRight += '<tr alt="' + memberAssignments[i].user_sys_id + '"><td class="right">'
                + '<label class="font-normal ' + classMaTotal + '">' + memberAssignments[i].total_plan_man_days.toFixed(1) + '</label> <label>人日</label>'
                + '</td></tr>';
            htmlMaSalesRight += '<tr alt="' + memberAssignments[i].user_sys_id + '"><td class="right">'
                + '<div class="div-sales"><div><label class="font-normal money ' + classMaSalesTotal + '">' + totalIndividualSalesPerMem + '</label>'
                + '<label>&nbsp;円 /&nbsp;</label></div><div><label class="font-normal plan-cost-by-user ' + classPlanCostTotal + '">' + PMS.utility.convertIntToMoney(totalPlanCostPerMem) + '</label><label>&nbsp;円</label></div></div>'
                + '</td></tr>';
        }

        $('table.tb-ma-left tr.tr-total').before(htmlMaLeft);
        $('table.tb-ma-sales-left tr.tr-total').before(htmlMaSalesLeft);
        $('table.tb-ma-center tr.tr-total').before(htmlMaCenter);
        $('table.tb-ma-sales-center tr.tr-total').before(htmlMaSalesCenter);
        $('table.tb-ma-right tr.tr-total').before(htmlMaRight);
        $('table.tb-ma-sales-right tr.tr-total').before(htmlMaSalesRight);
    }

    var $currentTotalIndividualSales = $parentForm.find('table.tb-ma-sales-right tr.tr-total label.total-ma-sales');
    var $currentTotalPlanCost = $parentForm.find('table.tb-ma-sales-right tr.tr-total label.total-plan-cost');
    var classTotalIndividualSales = classTotalPlanCost = "";

    if (currentProjectInfo.salesType == 0) {
        classTotalIndividualSales = compareValue($currentTotalIndividualSales, totalIndividualSales, true, true);
        classTotalPlanCost = compareValue($currentTotalPlanCost, totalPlanCost, true, true);
    }

    $('#totalIndividualSales, table.tb-ma-sales-right tr.tr-total label.total-ma-sales').text(totalIndividualSales).addClass(classTotalIndividualSales);
    $('table.tb-ma-sales-right tr.tr-total label.total-plan-cost').text(totalPlanCost).addClass(classTotalPlanCost);
}

// Bind data for payment list
function bindPaymentList(targetTimes, payments, paymentDetails) {
    var totalPayment = 0;

    if (payments.length > 0) {
        var htmlLeft = '';
        var htmlCenter = '';
        var htmlRight = '';
        var colLength = targetTimes.length;

        for (var i = 0; i < payments.length; i++) {
            var classHistory, classPaymentTotal = "";
            var $current = $parentForm.find('input.sc-id[value="' + payments[i].customer_id + '"]');

            if (currentProjectInfo.salesType == 0)
                classHistory = $current.length > 0 ? "" : "reduce";

            htmlLeft += "<tr alt='" + payments[i].customer_id + "'>"
                + "<td class='td-payment-name'><div class='text-overflow " + classHistory + "' title='" + payments[i].customer_name + "'>" + PMS.utility.htmlEncode(payments[i].customer_name) + "</div></td>"
                + "<td class='td-payment-pic-name'><div class='text-overflow " + classHistory + "' title='" + payments[i].charge_person_name + "'>" + PMS.utility.htmlEncode(payments[i].charge_person_name) + "</div>"
                + "</td></tr>";
            htmlCenter += '<tr alt="' + payments[i].customer_id + '">';

            for (var j = 0; j < colLength; j++) {
                var index = i * colLength + j;
                var classChange = "";

                if (currentProjectInfo.salesType == 0) {
                    if ($current.length > 0) {
                        var $currentPayment = $parentForm.find('input.payment-amount[alt="' + targetTimes[j] + '"][id="' + payments[i].customer_id + '"]');
                        classChange = compareValue($currentPayment, paymentDetails[index].amount, false, true);
                    } else {
                        classChange = classHistory;
                    }
                }

                htmlCenter += '<td class="right">'
                    + '<label class="font-normal money payment-amount ' + classChange + '" alt="' + targetTimes[j] + '">'
                    + paymentDetails[index].amount + '</label> '
                    + '<label>円</label></td>';
            }

            if (currentProjectInfo.salesType == 0) {
                if ($current.length > 0) {
                    var $currentPaymentTotal = $parentForm.find('table.tb-sc-right label[id="' + payments[i].customer_id + '"]');
                    classPaymentTotal = compareValue($currentPaymentTotal, payments[i].total_amount, true, true);
                } else {
                    classPaymentTotal = classHistory;
                }
            }

            htmlCenter += '</tr>';
            htmlRight += '<tr id="' + payments[i].customer_id + '">'
                + '<td class="right">'
                + '<label class="font-normal lbl-money ' + classPaymentTotal + '">' + payments[i].total_amount + '</label> '
                + '<label>円</label></td></tr>';

            totalPayment += parseInt(payments[i].total_amount);
        }

        $('table.tb-sc-left tr.tr-total').before(htmlLeft);
        $('table.tb-sc-center tr.tr-total').before(htmlCenter);
        $('table.tb-sc-right tr.tr-total').before(htmlRight);
    }

    var $currentTotalPayment = $parentForm.find('table.tb-sc-right tr.tr-total label.font-normal');
    var classTotalPayment = "";

    if (currentProjectInfo.salesType == 0)
        classTotalPayment = compareValue($currentTotalPayment, totalPayment, true, true);

    $('table.tb-sc-right tr.tr-total label.font-normal').text(totalPayment).addClass(classTotalPayment);
}

// Bind data for overhead cost list
function bindOverheadCostList(targetTimes, overheadCosts, overheadCostDetails) {
    var totalOverheadCost = 0;

    if (overheadCosts.length > 0) {
        var colLength = targetTimes.length;

        for (var i = 0; i < overheadCosts.length; i++) {
            var htmlLeft = '';
            var htmlCenter = '';
            var htmlRight = '';
            var classHistory, classOverheadCostTotal = "";
            var isValid = true;
            var $current = $parentForm.find('input.ovc-id[value="' + overheadCosts[i].detail_no + '"]');

            if (currentProjectInfo.salesType == 0)
                classHistory = $current.length > 0 ? "" : "reduce";

            htmlLeft += "<tr alt='" + overheadCosts[i].detail_no + "'>"
                + "<td class='td-ovc-type'><div class='text-overflow " + classHistory + "' title='" + overheadCosts[i].type_name + "'>" + PMS.utility.htmlEncode(overheadCosts[i].type_name) + "</div></td>"
                + "<td class='td-ovc-detail'><div class='text-overflow " + classHistory + "' title='" + overheadCosts[i].overhead_cost_detail + "'>" + PMS.utility.htmlEncode(overheadCosts[i].overhead_cost_detail) + "</div></td>"
                + "<td class='td-ovc-pic'><div class='text-overflow " + classHistory + "' title='" + overheadCosts[i].charge_person_name + "'>" + PMS.utility.htmlEncode(overheadCosts[i].charge_person_name) + "</div></td>"
                + "</tr>";
            htmlCenter += '<tr alt="' + overheadCosts[i].detail_no + '">';

            for (var j = 0; j < colLength; j++) {
                var index = i * colLength + j;
                var classChange = "";

                if (typeof (overheadCostDetails[index]) == 'undefined') {
                    isValid = false;
                    break;
                }

                if (currentProjectInfo.salesType == 0) {
                    if ($current.length > 0) {
                        var $currentOverheadCost = $parentForm.find('input.ovc-amount[alt="' + targetTimes[j] + '"][id="' + overheadCosts[i].detail_no + '"]');
                        classChange = compareValue($currentOverheadCost, overheadCostDetails[index].amount, false, true);
                    } else {
                        classChange = classHistory;
                    }
                }

                htmlCenter += '<td class="right">'
                    + '<label class="font-normal money ovc-amount ' + classChange + '" alt="' + targetTimes[j] + '">'
                    + overheadCostDetails[index].amount + '</label> '
                    + '<label>円</label></td>';
            }

            if (!isValid)
                break;

            if (currentProjectInfo.salesType == 0) {
                if ($current.length > 0) {
                    var $currentOverheadCostTotal = $parentForm.find('table.tb-ovc-right label[id="' + overheadCosts[i].detail_no + '"]');
                    classOverheadCostTotal = compareValue($currentOverheadCostTotal, overheadCosts[i].total_amount, true, true);
                } else {
                    classOverheadCostTotal = classHistory;
                }
            }

            htmlCenter += '</tr>';
            htmlRight += '<tr id="' + overheadCosts[i].detail_no + '">'
                + '<td class="right">'
                + '<label class="font-normal lbl-money ' + classOverheadCostTotal + '">' + overheadCosts[i].total_amount + '</label> '
                + '<label>円</label></td></tr>';

            if (isValid) {
                totalOverheadCost += parseInt(overheadCosts[i].total_amount);
                $('table.tb-ovc-left tr.tr-total').before(htmlLeft);
                $('table.tb-ovc-center tr.tr-total').before(htmlCenter);
                $('table.tb-ovc-right tr.tr-total').before(htmlRight);
            }
        }
    }

    var $currentOverheadCostPayment = $parentForm.find('table.tb-ovc-right tr.tr-total label.font-normal');
    var classTotalOverheadCost = "";

    if (currentProjectInfo.salesType == 0)
        classTotalOverheadCost = compareValue($currentOverheadCostPayment, totalOverheadCost, true, true);

    $('table.tb-ovc-right tr.tr-total label.font-normal').text(totalOverheadCost).addClass(classTotalOverheadCost);
}

// Bind data total by month on table list
function bindTotalByTargetTime(targetTimes) {
    for (var i = 0; i < targetTimes.length; i++) {
        var totalEstimate = 0;
        var totalIndividualSales = 0;
        var totalPlanCostOfMonth = 0;
        var totalPayment = 0;
        var totalOverheadCost = 0;
        var classMaTotal, classMaSalesTotal, classPlanCostOfMonthTotal, classPaymentTotal, classOverheadCostTotal = "";
        var $currentMaTotal = $parentForm.find('table.tb-ma-center label[name="' + targetTimes[i] + '"]');
        var $currentMaSalesTotal = $parentForm.find('table.tb-ma-sales-center label.ma-sales-by-month[name="' + targetTimes[i] + '"]');
        var $currentPlanCostOfMonthTotal = $parentForm.find('table.tb-ma-sales-center label.plan-cost-by-month[name="' + targetTimes[i] + '"]');
        var $currentPaymentTotal = $parentForm.find('table.tb-sc-center label[name="' + targetTimes[i] + '"]');
        var $currentOverheadCostTotal = $parentForm.find('table.tb-ovc-center label[name="' + targetTimes[i] + '"]');

        $('label.ma-man-days[alt="' + targetTimes[i] + '"]').each(function () {
            var planManDay = PMS.utility.validDecimalNumeric($(this).text()) ? parseFloat($(this).text()) : 0;

            totalEstimate += planManDay;
        });

        $('label.ma-sales[alt="' + targetTimes[i] + '"]').each(function () {
            totalIndividualSales += PMS.utility.convertMoneyToInt($(this).text());
        });

        $('label.plan-cost[alt="' + targetTimes[i] + '"]').each(function () {
            totalPlanCostOfMonth += PMS.utility.convertMoneyToInt($(this).text());
        });

        //var planCostOfMonth = '<label>/&nbsp;</label><label class="font-normal plan-cost-by-month money" name ="'+targetTimes[i]+'">' + totalPlanCostOfMonth + '</label><label>&nbsp;円</label>';

        //$('table.tb-ma-sales-center tr.tr-total td[headers="' + targetTimes[i] + '"]').append(planCostOfMonth);


        $('label.payment-amount[alt="' + targetTimes[i] + '"]').each(function () {
            totalPayment += PMS.utility.convertMoneyToInt($(this).text());
        });

        $('label.ovc-amount[alt="' + targetTimes[i] + '"]').each(function () {
            totalOverheadCost += PMS.utility.convertMoneyToInt($(this).text());
        });

        if (currentProjectInfo.salesType == 0) {
            classMaTotal = compareValue($currentMaTotal, parseFloat(totalEstimate.toFixed(1)), true, false);
            classMaSalesTotal = compareValue($currentMaSalesTotal, totalIndividualSales, true, true);
            classPlanCostOfMonthTotal = compareValue($currentPlanCostOfMonthTotal, totalPlanCostOfMonth, true, true);
            classPaymentTotal = compareValue($currentPaymentTotal, totalPayment, true, true);
            classOverheadCostTotal = compareValue($currentOverheadCostTotal, totalOverheadCost, true, true);
        }

        $('table.tb-ma-center label.font-normal[name="' + targetTimes[i] + '"]')
            .text(totalEstimate.toFixed(1))
            .addClass(classMaTotal);

        $('table.tb-ma-sales-center label.ma-sales-by-month[name="' + targetTimes[i] + '"]')
            .text(PMS.utility.convertIntToMoney(totalIndividualSales))
            .addClass(classMaSalesTotal);

        $('table.tb-ma-sales-center label.plan-cost-by-month[name="' + targetTimes[i] + '"]')
            .text(PMS.utility.convertIntToMoney(totalPlanCostOfMonth))
            .addClass(classPlanCostOfMonthTotal);

        $('table.tb-sc-center label.font-normal[name="' + targetTimes[i] + '"]')
            .text(PMS.utility.convertIntToMoney(totalPayment))
            .addClass(classPaymentTotal);

        $('table.tb-ovc-center label.font-normal[name="' + targetTimes[i] + '"]')
            .text(PMS.utility.convertIntToMoney(totalOverheadCost))
            .addClass(classOverheadCostTotal);
    }
}

// Event change history info
$(document).off('#dllHistory');
$(document).on('change', '#dllHistory', function () {
    loadProjectHistory();
});

// Bind data history of project
function loadProjectHistory() {
    if ($(opener.document).find('#hdnUserChangeValue').val() === 'true'
        || currentProjectInfo.projectID !== $(opener.document).find('#PROJECT_INFO_project_sys_id').val()) {
        PMS.utility.showMessageDialog('<i class="fa fa-warning"></i>', '編集中の内容があるため表示できません。', null, null, function () {
            window.close();
        });
        return;
    }
    $('.validation-summary-errors').remove();

    if ($('#dllHistory').children('option:selected').attr('aria-valuetext') == '1')
        PMS.utility.showClientError(['このプロジェクト情報は削除されました。'], '.color-description');

    var historyID = parseInt($('#dllHistory').val());

    if (historyID === 0) {
        $('#historyContent').hide();
    } else {
        var param = { projectID: currentProjectInfo.projectID, historyID: historyID };
        var data = PMS.utility.getDataByAjax('/PMS06001/ProjectInfoHistoryJson', param);

        // reset html to default display
        $('#historyContent').empty().append(defaultHTML);

        // bind footer by target time on table list
        PMS.utility.bindTotalCols(data.targetTimes, null, null);
        var classEstimate, classTotalSales, classTotalPayment, classTaxRate, classTotalPlanCost, classGrossProfit, classGrossProfitRate = "";
        var $currentEstimate = $parentForm.find('#PROJECT_INFO_estimate_man_days');
        var $currentTotalSales = $parentForm.find('#PROJECT_INFO_total_sales');
        var $currentTotalPayment = $parentForm.find('#PROJECT_INFO_total_payment');
        var $currentTaxRate = $parentForm.find('#PROJECT_INFO_tax_rate');
        var totalCost = data.projectInfo.total_sales - data.projectInfo.total_payment - data.projectInfo.gross_profit;
        var $currentTotalPlanCost = $parentSummary.find('#lblTotalCost');
        var $currentGrossProfit = $parentForm.find('#PROJECT_INFO_gross_profit');
        var currentGrossProfitRate = parseFloat($parentSummary.find('#lblExpectedGrossMargin').text().replace('%', ''));
        var historyGrossProfitRateTemp = (data.projectInfo.gross_profit / data.projectInfo.total_sales) * 100;
        var historyGrossProfitRate = $.isNumeric(historyGrossProfitRateTemp) ? historyGrossProfitRateTemp.toFixed(2) : '0';

        if (currentProjectInfo.salesType == 0) {
            classEstimate = compareValue($currentEstimate, data.projectInfo.estimate_man_days, false, false);
            classTotalSales = compareValue($currentTotalSales, data.projectInfo.total_sales, false, true);
            classTotalPayment = compareValue($currentTotalPayment, data.projectInfo.total_payment, false, true);
            classTaxRate = compareValue($currentTaxRate, (data.projectInfo.tax_rate * 100), false, true);
            classTotalPlanCost = compareValue($currentTotalPlanCost, totalCost, true, true);
            classGrossProfit = compareValue($currentGrossProfit, data.projectInfo.gross_profit, false, true, true);

            if (currentGrossProfitRate > parseFloat(historyGrossProfitRate))
                classGrossProfitRate = "rise";

            if (parseFloat(historyGrossProfitRate) > currentGrossProfitRate)
                classGrossProfitRate = "reduce";
        }

        // bind project info
        $('#insUser').text(data.projectInfo.ins_user);
        $('#estimate, table.tb-ma-right tr.tr-total label.font-normal')
            .text(data.projectInfo.estimate_man_days.toFixed(1))
            .addClass(classEstimate);
        $('#totalSales').text(data.projectInfo.total_sales).addClass(classTotalSales);
        $('#customer').text(data.projectInfo.customer_name);
        $('#endUser').text(data.projectInfo.end_user_name);
        $('#totalPayment').text(data.projectInfo.total_payment).addClass(classTotalPayment);
        $('#taxRate').text(parseInt(data.projectInfo.tax_rate * 100) + '%').addClass(classTaxRate);
        $('#totalCost').text(totalCost).addClass(classTotalPlanCost);
        $('#grossProfit').text(data.projectInfo.gross_profit).addClass(classGrossProfit);
        $('#grossProfitRate').text(historyGrossProfitRate + '%').addClass(classGrossProfitRate);

        // bind header by target time on table list
        $('table.tb-detail tr th.th-month').remove();
        for (var i = 0; i < data.targetTimes.length; i++) {
            $('table.tb-detail tr.month-colum').append('<th class="th-month">' + data.targetTimes[i] + '</th>');
        }

        $('table.tb-ma-sales-center tr.month-colum th.th-month').each(function () {
            $(this).append('<br>売上 / 予定原価');
        });

        // bind member assignment list
        bindMemberAssigmentList(data.targetTimes, data.memberAssignments, data.memberAssignmentDetails);

        // bind payment list
        bindPaymentList(data.targetTimes, data.payments, data.paymentDetails);

        // bind overhead cost list
        bindOverheadCostList(data.targetTimes, data.overheadCosts, data.overheadCostDetails);

        // bind total by target time
        bindTotalByTargetTime(data.targetTimes);

        // format string money
        PMS.utility.formatMoneyLabel();

        $('#historyContent').show();
    }
}
