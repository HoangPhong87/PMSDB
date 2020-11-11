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
        var oldValue = $(this).parent().parent("td").find(timeValue).val();
        var min = $(this).parent().parent().find(minuteInput).val();
        if (hour != "") {
            $(this).val(hour);
            
            if (isNaN(min) || parseInt(min) >= 60) {
                return;
            }
            $(this).parent().parent("td").find(timeValue).val(parseInt(hour) + parseInt(min == "" ? "00" : min) / 60.0);
            if (min == "") {
                $(this).parent().parent("td").find(".minute").val("00");
            }
        }
        else {
            if (min == "") {
                $(this).parent().parent("td").find(timeValue).val("");
            }
            else {
                $(this).parent().parent("td").find(timeValue).val(parseInt(min) / 60.0);
            }
        }
        if ($(this).parent().parent("td").find(timeValue).val() != oldValue) {
            $(this).parent().parent("td").find(inputChange).val('1');
        }
        updateTotal(check);
    });
}
//

function validateMinuteInput(control, minuteInput, timeValue, inputChange, check) {
    control.focusout(function () {
        var min = "";
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');

        if ($.isNumeric(value)) {
            min = formatNumber(value);
        }
        if (isNaN(min) || parseInt(min) >= 60) {
            $(this).css("background-color", "red");
            return;
        } else {
            $(this).removeAttr("style");
        }
        var oldValue = $(this).parent().parent("td").find(timeValue).val();
        var hour = $(this).parent().parent().find(minuteInput).val();
        
        if (min != "") {
            $(this).parent().parent("td").find(timeValue).val(parseInt(hour == "" ? "00" : hour) + parseInt("00" + min) / 60.0);
            $(this).val(min);
            if (hour == "") {
                $(this).parent().parent("td").find(".hour").val("00");
            }
        }
        else {
            $(this).parent().parent("td").find(timeValue).val(parseInt(hour == "" ? "00" : hour));
            if (hour == "") {
                $(this).parent().parent("td").find(timeValue).val("");
            }
            else {
                $(this).parent().parent("td").find(timeValue).val(parseInt(hour));
            }
        }
        if ($(this).parent().parent("td").find(timeValue).val() != oldValue) {
            $(this).parent().parent("td").find(inputChange).val('1');
        }
        updateTotal(check);
    });
}

function validateHourInputProject(control, minuteInput, timeValue, inputChange, check) {
    control.focusout(function () {
        var hour = "";
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
        if ($(this).parent().parent().find(minuteInput).val() == "00") {
            hour = $.isNumeric(value) ? value : '';
            $(this).parent().parent().find(minuteInput).val('');
        }
        else if ($(this).parent().parent().find(minuteInput).val() != "00") {
            hour = $.isNumeric(value) ? value : '00';
        }
        else {
            hour = $.isNumeric(value) ? value : '';
        }
        if (hour != "" && $(this).parent().parent().find(minuteInput).val() == "") {
            $(this).parent().parent().find(minuteInput).val("00");
        }
        if ((hour == "00" || hour == "0") && ($(this).parent().parent().find(minuteInput).val() == "00" || $(this).parent().parent().find(minuteInput).val() == "00")) {
            $(this).parent().parent().find(minuteInput).val("");
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
            $(this).val(hour);
            var min = $(this).parent().parent().find(minuteInput).val();
            if (isNaN(min) || parseInt(min) >= 60) {
                return;
            }
            $(this).parent().parent("td").find(timeValue).val(timeValue).val(parseInt('00' + hour) + parseInt(min == "" ? "00" + min : min) / 60.0);
            $(this).parent().parent("td").children(inputChange).val("1");
            updateTotal(check);
        }
        else {
            hour = parseInt(hour == '' ? '00' : hour);
            $(this).val(formatNumber(hour));
            var min = $(this).parent().parent().find(minuteInput).val();
            if (isNaN(min) || parseInt(min) >= 60) {
                return;
            }
            $(this).parent().parent("td").find(timeValue).val(parseInt(hour) + parseInt(min == "" ? "00" + min : min) / 60.0);
            $(this).parent().parent("td").find(inputChange).val("1");
            updateTotal(check);
        }

    });
}
//

function validateMinuteInputProject(control, minuteInput, timeValue, inputChange, check) {
    control.focusout(function () {
        var min = "";
        var value = PMS.utility.Convert_JPCharacters(this.value, 'n');
        if ($(this).parent().parent().find(minuteInput).val() == "00") {
            min = $.isNumeric(value) ? value : '';
            $(this).parent().parent().find(minuteInput).val('');
        }
        else if ($(this).parent().parent().find(minuteInput).val() != "00") {
            min = $.isNumeric(value) ? value : '00';
        }
        else {
            min = $.isNumeric(value) ? value : '';
        }

        if (min != "" && $(this).parent().parent().find(minuteInput).val() == "") {
            $(this).parent().parent().find(minuteInput).val("00");
        }
        if ((min == "00" || min == "0") && ($(this).parent().parent().find(minuteInput).val() == "00" || $(this).parent().parent().find(minuteInput).val() == "0")) {
            $(this).parent().parent().find(minuteInput).val("");
            min = "";
            $(this).val(min);
        }
        if (isNaN(min) || parseInt(min) >= 60) {
            $(this).css("background-color", "red");
            return;
        } else {
            $(this).removeAttr("style");
        }
        if (min == "") {
            var hour = $(this).parent().parent().find(minuteInput).val();
            $(this).parent().parent("td").find(timeValue).val(parseInt(hour == "" ? "00" : hour) + parseInt("00" + min) / 60.0);
            $(this).parent().parent("td").find(inputChange).val("1");
            $(this).val(min);
            updateTotal(check);
        }
        else {
            min = parseInt(min == '' ? '00' : min);
            var hour = $(this).parent().parent().find(minuteInput).val();
            if (hour == "")
                return;
            $(this).parent().parent("td").find(timeValue).val(parseInt(hour) + parseInt(min) / 60.0);
            $(this).parent().parent("td").find(inputChange).val("1");
            $(this).val(formatNumber(min));
            updateTotal(check);
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
        var totalHourAllProject = 0;
        var totalMinuteAllProject = 0;

        // total by project
        $('.total-actual').parent('tr').children('input[id="ProjectId"]').each(function () {
            var totalHourByProject = 0;
            var totalMinuteByProject = 0;
            var projectId = $(this).val();
            $("td.actual-time" + '.' + projectId).each(function () {
                totalHourByProject += parseInt($(this).find('.hourInput').val() == '' ? '00' : $(this).find('.hourInput').val());
                totalMinuteByProject += parseInt($(this).find('.minuteInput').val() == '' ? '00' : $(this).find('.minuteInput').val());
            });

            if (totalMinuteByProject >= 60) {
                var tempHour = totalMinuteByProject / 60;
                var plusHour = parseInt(tempHour.toString().split('.')[0]);

                totalHourByProject += plusHour;
                totalMinuteByProject -= plusHour * 60;
            }

            totalHourAllProject += totalHourByProject;
            totalMinuteAllProject += totalMinuteByProject;

            var hour = formatNumber(totalHourByProject);
            var min = formatNumber(totalMinuteByProject);

            if ($('.tempTime' + projectId).length > 0) {
                $('.total-actual').parent('tr').find('.tempTime' + projectId).val(hour + '<span> : </span>' + min);
            } else {
                $('.total-actual').parent('tr').append("<input type='hidden' class='time_phase tempTime" + projectId + "'  value='" + hour + '<span> : </span>' + min + "' />");
            }
            $('.total-actual' + '.' + projectId).html($('.tempTime' + projectId).val())
        });

        if (totalMinuteAllProject >= 60) {
            var tempAllHour = totalMinuteAllProject / 60;
            var plusAllHour = parseInt(tempAllHour.toString().split('.')[0]);

            totalHourAllProject += plusAllHour;
            totalMinuteAllProject -= plusAllHour * 60;
        }

        // total all project
        var totalHour = formatNumber(totalHourAllProject);
        var totalMin = formatNumber(totalMinuteAllProject);

        $('.total-actual-time').html(totalHour + '<span> : </span>' + totalMin);
        $('.total-actual-time').append("<input type='hidden' class='tempTimeTotal'  value='" + totalHour + ':' + totalMin + "' />");

    }
    else { // update total actual work time
        var end_time = $("input.end_time").val() == '' ? '00' : $("input.end_time").val();
        if (end_time == "NaN") {
            end_time = "0";
        }
        if (end_time.length > 0)
            totalArrayActual = Math.round(parseFloat(end_time) * 60).toFixed();
        else
            totalArrayActual = 0;

        var start_time = $("input.start_time").val() == '' ? '00' : $("input.start_time").val();
        if (start_time == "NaN") {
            start_time = "0";
        }
        if (start_time.length > 0)
            totalArrayActual -= Math.round(parseFloat(start_time) * 60).toFixed();

        var rest_time = $("input.rest_time").val() == '' ? '00' : $("input.rest_time").val();
        if (rest_time == "NaN") {
            rest_time = "0";
        }
        if (rest_time.length > 0)
            totalArrayActual -= Math.round(parseFloat(rest_time) * 60).toFixed();

        totalArrayActual = Math.round(totalArrayActual / 60 * 100) / 100;
        var min_actual = Math.round(Math.floor(totalArrayActual) == 0 && totalArrayActual < 0
                                                    ? ((Math.floor(totalArrayActual) + 1) - totalArrayActual) * 60
                                                    : (totalArrayActual - Math.floor(totalArrayActual)) * 60);
        min_actual = min_actual != 0 && totalArrayActual < 0 ? 60 - min_actual : min_actual;
        var hour_actual = 0;
        if (min_actual != 0 && totalArrayActual < 0) {
            hour_actual = Math.floor(totalArrayActual) + 1;
        } else {
            hour_actual = Math.floor(totalArrayActual);
        }

        var resultHour = (hour_actual == 0 && totalArrayActual < 0 ? '-' + formatNumberTime(hour_actual) : formatNumberTime(hour_actual));
        var resultMin = formatNumber(min_actual);

        if (parseInt(resultMin) == 60) {
            resultMin = '00';
            resultHour = formatNumber(parseInt(resultHour) + 1);
        }

        $('.total').html(resultHour + '<span> : </span>' + resultMin);
        $('.total').append("<input type='hidden' class='actualTime' value='" + resultHour + ':' + resultMin + "' />");
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
validateHourInputProject($("input.hourInput"), "input.minuteInput", ".timeValue", "input.isChanged", true);
validateHourInput($("input.start_time_hour"), "input.start_time_minute", ".start_time", "input.isChanged", false);
validateHourInput($("input.end_time_hour"), "input.end_time_minute", ".end_time", "input.isChanged", false);
validateHourInput($("input.rest_time_hour"), "input.rest_time_minute", ".rest_time", "input.isChanged", false);
validateHourInput($("input.allowed_cost_time_hour"), "input.allowed_cost_time_minute", ".allowed_cost_time", "input.isChanged", false);

validateMinuteInputProject($("input.minuteInput"), "input.hourInput", ".timeValue", "input.isChanged", true);
validateMinuteInput($("input.start_time_minute"), "input.start_time_hour", ".start_time", "input.isChanged", false);
validateMinuteInput($("input.end_time_minute"), "input.end_time_hour", ".end_time", "input.isChanged", false);
validateMinuteInput($("input.rest_time_minute"), "input.rest_time_hour", ".rest_time", "input.isChanged", false);
validateMinuteInput($("input.allowed_cost_time_minute"), "input.allowed_cost_time_hour", ".allowed_cost_time", "input.isChanged", false);


PMS.utility.focusTextbox();
PMS.utility.imeControl($(".hour"), 'disable');
PMS.utility.imeControl($(".minute"), 'disable');
PMS.utility.imeControl($(".remark-content"), 'active');

//change value of checkbox attendance_type
$('select[name="AttendanceRecordInfor.attendance_type_id"]').change(function () {
    $(this).parent("td").children('input.attendance_type').val($(this).val());
    $(this).parent("td").children('input.isChanged').val("1");
});

//change value of checkbox attendance_type
$('.remark-content').change(function () {
    $(this).parent("td").children('input.isChanged').val("1");
});

updateTotal(true);
updateTotal(false);

if ($('input[name="registerType"]').val() == '0') {
    $(".tbl-actual-work-regist-new.actual-work tr td.actual-time").each(function (i) {
        $("input", $(this)).attr('readonly', true);

    });

    $("input.start_time_hour").attr('readonly', true);
    $("input.start_time_minute").attr('readonly', true);
    $("input.end_time_hour").attr('readonly', true);
    $("input.end_time_minute").attr('readonly', true);
    $("input.rest_time_hour").attr('readonly', true);
    $("input.rest_time_minute").attr('readonly', true);
    $("input.allowed_cost_time_hour").attr('readonly', true);
    $("input.allowed_cost_time_minute").attr('readonly', true);
    $("select").attr('disabled', 'disabled');
    $(".remark-content").attr('readonly', true);

    var btnUpdate = $('#btnUpdate');
    btnUpdate.attr('disabled', 'disabled');
    btnUpdate.removeClass('blue');
    btnUpdate.addClass('disabled');

    var btnClear = $('#btnClear');
    btnClear.attr('disabled', 'disabled');
    btnClear.addClass('disabled');
}