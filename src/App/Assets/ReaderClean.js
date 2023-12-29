// 隐藏指定元素
function hideElements(selector) {
    var elements = document.querySelectorAll(selector);
    for (var i = 0; i < elements.length; i++) {
        elements[i].style.display = 'none';
    }
}

// 修改img的data-src属性为src属性
function modifyImgSrc() {
    var images = document.getElementsByTagName('img');
    for (var i = 0; i < images.length; i++) {
        var dataSrc = images[i].getAttribute('data-src');
        if (dataSrc) {
            images[i].setAttribute('src', dataSrc);
        }
    }
}

// 设置段落样式
function setParagraphStyles() {
    var paragraphs = document.querySelectorAll('#read-app-scroll p');
    var headings = document.querySelectorAll('#read-app-scroll h1, #read-app-scroll h2, #read-app-scroll h3, #read-app-scroll h4, #read-app-scroll h5, #read-app-scroll h6');

    for (var i = 0; i < paragraphs.length; i++) {
        paragraphs[i].style.marginBottom = '15px';
        paragraphs[i].style.fontSize = '16px';
    }

    for (var j = 0; j < headings.length; j++) {
        var headingLevel = parseInt(headings[j].tagName.charAt(1));
        var margin = 20 - (headingLevel * 2);
        headings[j].style.marginBottom = margin + 'px';
    }
}

function setAppearance() {
    var scrollbarStyle = `
  *::-webkit-scrollbar {
  width: 12px;
}

*::-webkit-scrollbar-track {
  background: $body-background$;
}

*::-webkit-scrollbar-thumb {
  background-color: $scroll-foreground$;
  border-radius: 20px;
  border: 3px solid $body-background$;
}

body {
    background: $body-background$ !important;
    margin-top: 20px !important;
    margin-left: 16px !important;
}
`;

    var styleElement = document.createElement('style');
    styleElement.innerHTML = scrollbarStyle;
    document.head.appendChild(styleElement);
}

(function () {
    // 隐藏指定元素
    hideElements('.n-img-mask');
    hideElements('.v-affix');
    hideElements('.read-interaction-info');
    hideElements('#readRecommendInfo');
    hideElements('.read-act-box');
    hideElements('#read-comment-box');
    hideElements('.outside-tabbar-box');
    hideElements('#read-title-bar');
    hideElements('.banner-img-hold');

    // 修改img的data-src属性为src属性
    modifyImgSrc();

    // 设置段落样式
    setParagraphStyles();

    setAppearance();
})()