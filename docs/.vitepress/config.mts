import { defineConfig } from 'vitepress'
import { shared } from './shared.mjs'
import { zh } from './zh.mjs'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  ...shared,
  locales: {
    root: { label: '简体中文', ...zh },
    }
  })
