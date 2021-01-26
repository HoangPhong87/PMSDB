
// Window 名を設定
function SetWindowName(name) {
    sessionStorage.setItem('WindowName', name);
    window.name = name;
}

// 画面遷移時に Window 名を Cookie に設定
$(function () {
    $(window).on("beforeunload", function (e) {
        var name = sessionStorage.getItem('WindowName');
        if (name != null) {
            document.cookie = 'WindowName=' + name + '; path=/';
        }
    });
});
