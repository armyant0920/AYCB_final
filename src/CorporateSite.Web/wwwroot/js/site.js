// 公開頁面全域 JavaScript — 由美工維護

// 手機選單漢堡
(function () {
    var btn = document.querySelector('.menu-toggle');
    var menu = document.querySelector('.menu');
    if (btn && menu) {
        btn.addEventListener('click', function () {
            menu.classList.toggle('open');
        });
    }
})();
