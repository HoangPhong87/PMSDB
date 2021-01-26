//Validate the hour field value
function validateHourInput(control, minuteInput, timeValue, inputChange, check) {
    control.focusout(function () {
        var hour = "";
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
        if ($.isNumeric(value)) {
            hour = formatNumber(value);
        }
        if (isNaN(hour)) {
            $(this).css("background-color", "red");
            return;
        } else {
            $(this).removeAttr("style");
        }
        var oldValue = $(this).parent("td").children(timeValue).val();
        var min = $(this).siblings(minuteInput).val();
        if (isNaN(min) || parseInt(min) >= 60) {
            return;
        }

        if (hour != "") {
            $(this).parent("td").children(timeValue).val(parseInt(hour) + parseInt(min == "" ? "00" : min) / 60.0);
            $(this).val(hour);
        }
        else {
            if (min == "") {
                $(this).parent("td").children(timeValue).val("");
            }
            else {
                $(this).parent("td").children(timeValue).val(parseInt(min) / 60.0);
            }
        }
        if ($(this).parent("td").children(timeValue).val() != oldValue) {
            $(this).parent("td").children(timeValue).attr(inputChange, '1');
        }
        updateTotal(check);
        checkRegisterTemp();
    });
}

//Validate the minute field value
function validateMinuteInput(control, minuteInput, timeValue, inputChange, check) {
    control.focusout(function () {
        var min = "";
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
        if ($.isNumeric(value)) {
            min = formatNumber(value);
        }
        if ((isNaN(min) || parseInt(min) >= 60)) {
            $(this).css("background-color", "red");
            return;
        } else {
            $(this).removeAttr("style");
        }
        var oldValue = $(this).parent("td").children(timeValue).val();
        var hour = $(this).siblings(minuteInput).val();

        if (min != "") {
            $(this).parent("td").children(timeValue).val(parseInt(hour == "" ? "00" : hour) + parseInt(min) / 60.0);
            $(this).val(min);
        }
        else {
            if (hour == "") {
                $(this).parent("td").children(timeValue).val("");
            }
            else {
                $(this).parent("td").children(timeValue).val(parseInt(hour == "" ? "00" : hour));
            }
        }
        if ($(this).parent("td").children(timeValue).val() != oldValue) {
            $(this).parent("td").children(timeValue).attr(inputChange, '1');
        }
        updateTotal(check);
        checkRegisterTemp();
    });
}

function validateHourInputProject(control, minuteInput, timeValue, inputChange, check) {
    control.focusout(function () {
        var hour = "";
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
        if ($(this).siblings(minuteInput).val() == "00") {
            hour = $.isNumeric(value) ? value : '';
            $(this).siblings(minuteInput).val('');
        }
        else if ($(this).siblings(minuteInput).val() != "00") {
            hour = $.isNumeric(value) ? value : '00';
        }
        else {
            hour = $.isNumeric(value) ? value : '';
        }
        if (hour != "" && $(this).siblings(minuteInput).val() == "") {
            $(this).siblings(minuteInput).val("00");
        }
        if (hour == "00" && $(this).siblings(minuteInput).val() == "00") {
            $(this).siblings(minuteInput).val("");
            hour = "";
            $(this).val(hour);
        }
        if (isNaN(hour)) {
            $(this).css("background-color", "red");
            return;
        } else {
            $(this).removeAttr("style");
        }
        if (hour == "") {
            var min = $(this).siblings(minuteInput).val();
            if (isNaN(min) || parseInt(min) >= 60) {
                return;
            }
            $(this).parent("td").children(timeValue).val(parseInt("00" + hour) + parseInt(min == "" ? "00" : min) / 60.0);
            $(this).parent("td").children(timeValue).attr(inputChange, '1');
            $(this).val(hour);
            updateTotal(check);
            checkRegisterTemp();
        }
        else {
            hour = parseInt(hour);
            var min = $(this).siblings(minuteInput).val();
            if (isNaN(min) || parseInt(min) >= 60) {
                return;
            }
            $(this).parent("td").children(timeValue).val(parseInt(hour) + parseInt(min == "" ? "00" : min) / 60.0);
            $(this).parent("td").children(timeValue).attr(inputChange, '1');
            $(this).val(formatNumber(hour));
            updateTotal(check);
            checkRegisterTemp();
        }
    });
}

//Validate the minute field value
function validateMinuteInputProject(control, minuteInput, timeValue, inputChange, check) {
    control.focusout(function () {
        var min = "";
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
        if ($(this).siblings(minuteInput).val() == "00") {
            min = $.isNumeric(value) ? value : '';
            $(this).siblings(minuteInput).val('');
        }
        else if ($(this).siblings(minuteInput).val() != "00") {
            min = $.isNumeric(value) ? value : '00';
        }
        else {
            min = $.isNumeric(value) ? value : '';
        }
        if (min != "" && $(this).siblings(minuteInput).val() == "") {
            $(this).siblings(minuteInput).val("00");
        }
        if (min == "00" && $(this).siblings(minuteInput).val() == "00") {
            $(this).siblings(minuteInput).val("");
            min = "";
            $(this).val(min);
        }
        if ((isNaN(min) || parseInt(min) >= 60)) {
            $(this).css("background-color", "red");
            return;
        } else {
            $(this).removeAttr("style");
        }
        if (min == "") {
            var hour = $(this).siblings(minuteInput).val();
            $(this).parent("td").children(timeValue).val(parseInt(hour == "" ? "00" : hour) + parseInt("00" + min) / 60.0);
            $(this).parent("td").children(timeValue).attr(inputChange, '1');

            $(this).val(min);
            updateTotal(check);
            checkRegisterTemp();
        }
        else {
            min = parseInt(min == '' ? '00' : min);
            var hour = $(this).siblings(minuteInput).val();
            if (hour == "")
                return;
            $(this).parent("td").children(timeValue).val(parseInt(hour) + parseInt(min) / 60.0);
            $(this).parent("td").children(timeValue).attr(inputChange, '1');

            $(this).val(formatNumber(min));
            updateTotal(check);
            checkRegisterTemp();
        }

    });
}
//Format the time value to the form 00:00
function formatNumber(x) {
    x = parseInt(x);
    if (x == "" || x == 0)
        return "00";
    else if (x < 10)
        return '0' + x;
    else
        return '' + x;
}

function formatNumberTime(x) {
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

//Update the total row work time
function updateTotal(check) {
    //Array contain total work time of each day
    var totalArray = new Array();
    var totalArrayActual = new Array();

    if (check) { // update total work time
        $(".tbl-container-content tbody tr.tr-work-time").each(function (i) {
            $("td", $(this)).each(function (j) {
                var hourDay = parseInt($(this).find('.hourInput').val() == "" ? "00" : $(this).find('.hourInput').val());
                var hourMinute = parseInt($(this).find('.minuteInput').val() == "" ? "00" : $(this).find('.minuteInput').val());

                if (typeof (totalArray[j]) == 'undefined') {
                    var totalTime = {
                        hour: hourDay,
                        minute: hourMinute
                    }

                    totalArray.push(totalTime);
                } else {
                    totalArray[j].hour += hourDay;
                    totalArray[j].minute += hourMinute;
                }
            });
        });

        $(".tbl-container-content  tr.resultRow td.total").each(function (k) {
            var totalHourByProject = totalArray[k].hour;
            var totalMinuteByProject = totalArray[k].minute;

            if (totalMinuteByProject >= 60) {
                var tempHour = totalMinuteByProject / 60;
                var plusHour = parseInt(tempHour.toString().split('.')[0]);

                totalHourByProject += plusHour;
                totalMinuteByProject -= plusHour * 60;
            }

            var hour = formatNumber(totalHourByProject);
            var min = formatNumber(totalMinuteByProject);

            $(this).html(hour + '<span> : </span>' + min);
            $(this).append("<input type='hidden' class='tempTime'  value='" + hour + ':' + min + "' />");
        });
    } else { // update total actual work time
        $(".tbl-container-content  tr.tr-end-time td").each(function (i) {
            var end_time = $(this).children("input.end_time").val();
            if (end_time == "NaN") {
                end_time = "0";
            }
            if (end_time.length > 0)
                totalArrayActual[i] = (parseFloat(end_time) * 60).toFixed();
            else
                totalArrayActual[i] = 0;
        });

        $(".tbl-container-content  tr.tr-start-time td").each(function (i) {
            var start_time = $(this).children("input.start_time").val();
            if (start_time == "NaN") {
                start_time = "0";
            }
            if (start_time.length > 0)
                totalArrayActual[i] -= (parseFloat(start_time) * 60).toFixed();
        });

        $(".tbl-container-content  tr.tr-rest-time td").each(function (i) {
            var rest_time = $(this).children("input.rest_time").val();
            if (rest_time == "NaN") {
                rest_time = "0";
            }
            if (rest_time.length > 0)
                totalArrayActual[i] -= (parseFloat(rest_time) * 60).toFixed();
        });

        $(".tbl-container-content  tr.resultActualRow td").each(function (k) {
            totalArrayActual[k] = Math.round(totalArrayActual[k] / 60 * 100) / 100;
            var min_actual = Math.round(Math.floor(totalArrayActual[k]) == 0 && totalArrayActual[k] < 0
                ? ((Math.floor(totalArrayActual[k]) + 1) - totalArrayActual[k]) * 60
                : (totalArrayActual[k] - Math.floor(totalArrayActual[k])) * 60);
            min_actual = min_actual != 0 && totalArrayActual[k] < 0 ? 60 - min_actual : min_actual;
            var hour_actual = 0;
            if (min_actual != 0 && totalArrayActual[k] < 0) {
                hour_actual = Math.floor(totalArrayActual[k]) + 1;
            } else {
                hour_actual = Math.floor(totalArrayActual[k]);
            }

            var resultHour = (hour_actual == 0 && totalArrayActual[k] < 0 ? '-' + formatNumberTime(hour_actual) : formatNumberTime(hour_actual));
            var resultMin = formatNumber(min_actual);


            if (parseInt(resultMin) == 60) {
                resultMin = '00';
                resultHour = formatNumber(parseInt(resultHour) + 1);
            }

            $(this).html(resultHour + '<span> : </span>' + resultMin);
            $(this).append("<input type='hidden' class='actualTime' value='" + resultHour + ':' + resultMin + "' />");
        });
    }
}

function checkInputNumber(control) {
    control.on("change keyup", function (e) {
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
        var regX = /[^0-9]/g;
        if (regX.test(value)) {
            $(this).val(value.replace(regX, ''));
        }
    });
}

checkInputNumber($("input.hour"));
checkInputNumber($("input.minute"));

//Validate the hour field value start_time_hour
validateHourInputProject($(".tbl-container-content tbody tr.tr-work-time td input.hourInput"), "input.minuteInput", ".timeValue", "ischanged", true);
validateHourInput($(".tbl-container-content  tr.tr-start-time td input.start_time_hour"), "input.start_time_minute", ".start_time", "ischanged", false);
validateHourInput($(".tbl-container-content  tr.tr-end-time td input.end_time_hour"), "input.end_time_minute", ".end_time", "isChanged", false);
validateHourInput($(".tbl-container-content  tr.tr-rest-time td input.rest_time_hour"), "input.rest_time_minute", ".rest_time", "isChanged", false);
validateHourInput($(".tbl-container-content  tr.tr-allowed-cost-time td input.allowed_cost_time_hour"), "input.allowed_cost_time_minute", ".allowed_cost_time", "isChanged", false);

validateMinuteInputProject($(".tbl-container-content tbody tr.tr-work-time td input.minuteInput"), "input.hourInput", ".timeValue", "isChanged", true);
validateMinuteInput($(".tbl-container-content  tr.tr-start-time td input.start_time_minute"), "input.start_time_hour", ".start_time", "isChanged", false);
validateMinuteInput($(".tbl-container-content  tr.tr-end-time td input.end_time_minute"), "input.end_time_hour", ".end_time", "isChanged", false);
validateMinuteInput($(".tbl-container-content  tr.tr-rest-time td input.rest_time_minute"), "input.rest_time_hour", ".rest_time", "isChanged", false);
validateMinuteInput($(".tbl-container-content  tr.tr-allowed-cost-time td input.allowed_cost_time_minute"), "input.allowed_cost_time_hour", ".allowed_cost_time", "isChanged", false);

PMS.utility.focusTextbox();
PMS.utility.imeControl($(".hour"), 'disable');
PMS.utility.imeControl($(".minute"), 'disable');

//change value of checkbox attendance_type
$('select[name="attendance_type"]').change(function () {
    $(this).parent("td").children('input.attendance_type').val($(this).val());
    $(this).parent("td").children('input.attendance_type').attr('ischanged', '1');

});

updateTotal(true);
updateTotal(false);

if ($('input[name="actualWorkReadonly"]').length > 0) {
    var hour = "00";
    var min = "00";
    $(".tbl-container-content  tr.resultRow td.total").each(function () {
        $(this).html(hour + '<span> : </span>' + min);
        $(this).append("<input type='hidden' class='tempTime'  value='" + hour + ':' + min + "' />");
    });
}
checkRegisterTemp();


//Check Register temp
function checkRegisterTemp() {
    if ($('input[name="actualWorkReadonly"]').length == 0) {
        //var check = true;
        $(".tbl-container-content tr.resultRow td").each(function (index) {
            var totalTemp = $(this).children('input.tempTime').val();
            var totalActual = $($(".tbl-container-content  tr.resultActualRow td")[index]).children('input.actualTime').val();
            if (totalTemp !== totalActual) {
                //check = false;
                $(this).css('background-color', '#ffd988');
            } else {
                $(this).removeAttr("style");
            }
        });
    }
}

if ($('input[id="regist_type"]').val() == '0') {
    $("input.hour").attr('readonly', true);
    $("input.minute").attr('readonly', true);

    $("input.start_time_hour").attr('readonly', true);
    $("input.start_time_minute").attr('readonly', true);
    $("input.end_time_hour").attr('readonly', true);
    $("input.end_time_minute").attr('readonly', true);
    $("input.rest_time_hour").attr('readonly', true);
    $("input.rest_time_minute").attr('readonly', true);
    $("input.allowed_cost_time_hour").attr('readonly', true);
    $("input.allowed_cost_time_minute").attr('readonly', true);
    $("select").attr('disabled', 'disabled');

    var btnUpdate = $('#btnUpdate');
    btnUpdate.attr('disabled', 'disabled');
    btnUpdate.removeClass('blue');
    btnUpdate.addClass('disabled');
}




