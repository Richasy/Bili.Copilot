---
# https://vitepress.dev/reference/default-theme-home-page
layout: home

hero:
  name: 哔哩助理
  text: 基于 Windows App SDK 和 AI 构建的 BiliBili 第三方应用
  tagline: 提供 Windows 原生体验
  actions:
    - theme: brand
      text: 安装应用
      link: /install
    - theme: alt
      text: 关于此应用
      link: /about
    - theme: alt
      text: Github
      link: https://github.com/Richasy/Bili.Copilot
  image:
    src: /logo-big.png
    alt: 哔哩助理

features:
  - icon: 📺
    title: 本机播放
    details: 使用本机硬件的解码能力播放视频。
  - icon: 🍻
    title: 功能丰富
    details: 涵盖绝大部分常用功能，满足大多数人的浏览需求。
  - icon: 🤖
    title: AI 赋能
    details: 支持20余种AI服务，视频一键总结，即问即答。
---
<style>
:root {
  --vp-home-hero-name-color: transparent;
  --vp-home-hero-name-background: -webkit-linear-gradient(-12deg, #DFC3FC 20%, #FD99D0 60%, #FE508E);

  --vp-home-hero-image-background-image: linear-gradient(160deg, #F3F3F3 50%, #FF79B2 60%, #FEB5FE 80%);
  --vp-home-hero-image-filter: blur(44px);
}

@media (min-width: 640px) {
  :root {
    --vp-home-hero-image-filter: blur(56px);
  }
}

@media (min-width: 960px) {
  :root {
    --vp-home-hero-image-filter: blur(68px);
  }
}
</style>