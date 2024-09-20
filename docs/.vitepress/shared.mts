import { defineConfig } from 'vitepress'

export const shared = defineConfig({
    title: '哔哩助理',
  
    lastUpdated: true,
    cleanUrls: true,
    metaChunk: true,

    markdown: {
        codeTransformers: [
          {
            postprocess(code) {
              return code.replace(/\[\!\!code/g, '[!code')
            }
          }
        ]
      },

    head: [
      ['link', { rel: 'icon', type: 'image/svg+xml', href: '/logo-small.svg' }],
      ['link', { rel: 'icon', type: 'image/png', href: '/logo-small.png' }],
      ['meta', { name: 'theme-color', content: '#FF578D' }],
      ['meta', { property: 'og:type', content: 'website' }],
      ['meta', { property: 'og:locale', content: 'zh' }],
      ['meta', { property: 'og:title', content: '哔哩助理 | BiliBili 第三方桌面客户端' }],
      ['meta', { property: 'og:site_name', content: '哔哩助理' }],
      ['meta', { property: 'og:url', content: 'https://bili.richasy.net/' }],
    ],

    themeConfig: {
        logo: { src: '/logo-small.svg', width: 24, height: 24 },
    
        socialLinks: [
          { icon: 'github', link: 'https://github.com/Richasy/Bili.Copilot' }
        ],
      }
})  