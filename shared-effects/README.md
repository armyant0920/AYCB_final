# Shared Effects — 共用前端特效

從本 JSP 專案抽出、改寫為「與框架無關」的純前端特效，移除 jQuery / 第三方套件依賴，
可直接套用到 **.NET MVC** 網站或任何 HTML 頁面。

> 這些特效原本散落在各 JSP / Home.js / preview.js / inside.css / video.css 中，
> 現整理成獨立、可重複使用的 CSS class 與 JS 類別。

## 內容物

| 特效 | 檔案 | 原始來源 | 是否需要 JS |
| --- | --- | --- | --- |
| 圖片縮放（hover 放大） | `css/effects.css` + `js/image-zoom.js`（選用） | `inside.css` / `video.css` | 否，純 CSS 即可 |
| 圖片輪播（跑馬燈式 banner） | `css/effects.css` + `js/carousel.js` | `Home.js` / `Home.css` | 是 |
| 文字跑馬燈 | `css/effects.css` + `js/marquee.js`（選用） | 新增通用版 | 否，純 CSS 即可 |
| 圖片上傳即時預覽 | `js/image-preview.js` | `preview.js` | 是 |

所有 JS 皆為**無依賴的原生 JavaScript**，且同時支援 `window` 全域與 CommonJS。

---

## 在 .NET MVC 專案中使用

1. 把 `css/effects.css` 複製到 `wwwroot/css/`，把 `js/` 內需要的檔案複製到 `wwwroot/js/`。
2. 在 `_Layout.cshtml`（或個別 View）引入：

```html
<link rel="stylesheet" href="~/css/effects.css" />
<!-- 依需求引入，未用到的可省略 -->
<script src="~/js/carousel.js"></script>
<script src="~/js/image-preview.js"></script>
<script src="~/js/marquee.js"></script>
<script src="~/js/image-zoom.js"></script>
```

3. 依下方範例在 View 中加上對應的 HTML 與初始化程式。

---

## 1. 圖片縮放特效

純 CSS，最簡單：

```html
<img class="fx-zoom" src="~/images/product.jpg" />

<!-- 想改放大倍率（預設 1.25） -->
<img class="fx-zoom" style="--fx-zoom-scale:1.4" src="~/images/product.jpg" />

<!-- 想避免放大時溢出框線，外層包一個 frame -->
<span class="fx-zoom-frame">
    <img class="fx-zoom" src="~/images/product.jpg" />
</span>
```

或用 JS 一次套用到一批元素：

```html
<script src="~/js/image-zoom.js"></script>
<script> FxImageZoom.apply('.product-pic', 1.3); </script>
```

## 2. 圖片輪播

```html
<div class="fx-carousel" id="banner">
    <ul class="fx-carousel__slides">
        <li><img src="~/images/a.jpg"></li>
        <li><img src="~/images/b.jpg"></li>
        <li><img src="~/images/c.jpg"></li>
    </ul>
    <ul class="fx-carousel__dots"></ul>  <!-- 圓點自動產生 -->
    <button class="fx-carousel__btn fx-carousel__btn--prev">&#10094;</button>
    <button class="fx-carousel__btn fx-carousel__btn--next">&#10095;</button>
</div>

<script src="~/js/carousel.js"></script>
<script>
    new FxCarousel('#banner', { interval: 3000 });   // 每 3 秒換一張
</script>
```

可用參數：`interval`（毫秒，預設 2000）、`autoplay`（預設 true）、`pauseOnHover`（預設 true）。

## 3. 文字跑馬燈

純 CSS：

```html
<div class="fx-marquee" style="--fx-marquee-duration:12s">
    <span>最新消息：歡迎光臨本網站！</span>
</div>
```

或用 JS（可動態帶入後端資料）：

```html
<div id="news"></div>
<script src="~/js/marquee.js"></script>
<script>
    new FxMarquee('#news', { text: '@ViewBag.Notice', duration: 12 });
</script>
```

## 4. 圖片上傳即時預覽

```html
<input type="file" id="imageFile" accept="image/*" />
<img id="imageDemo" alt="預覽圖" />

<script src="~/js/image-preview.js"></script>
<script>
    new FxImagePreview('#imageFile', '#imageDemo');
</script>
```

---

## 快速預覽

直接用瀏覽器打開 `demo.html` 即可看到全部四種特效的實際效果，不需任何後端。
