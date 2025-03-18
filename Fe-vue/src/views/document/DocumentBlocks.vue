<template>
  <div class="document-blocks">
    <div class="block">Mở đầu</div>
    <div
      class="block"
      v-for="(block, index) in firstBlocks"
      :key="index"
      :style="{ paddingLeft: block.Level != 0 ? block.Level * 20 + 'px' : '40px' }"
    >
      <div
        class="block__level"
        :style="{ backgroundColor: colors[block.Level - 1].color }"
        v-if="block.Level != 0"
      ></div>
      <div v-html="marked(block.Content)" class="markdown-container"></div>
    </div>
    <div class="block">Nội dung</div>
    <div
      class="block"
      v-for="(block, index) in contentBlocks"
      :key="index"
      :style="{ paddingLeft: block.Level != 0 ? block.Level * 20 + 'px' : '40px' }"
    >
      <div
        class="block__level"
        :style="{ backgroundColor: colors[block.Level - 1].color }"
        v-if="block.Level != 0"
      ></div>
      <div v-html="marked(block.Content)" class="markdown-container"></div>
    </div>
    <div class="block">Ký tên</div>
    <div
      class="block"
      v-for="(block, index) in signBlocks"
      :key="index"
      :style="{ paddingLeft: block.Level != 0 ? block.Level * 20 + 'px' : '40px' }"
    >
      <div
        class="block__level"
        :style="{ backgroundColor: colors[block.Level - 1].color }"
        v-if="block.Level != 0"
      ></div>
      <div v-html="marked(block.Content)" class="markdown-container"></div>
    </div>
    <div class="block">Khác</div>
    <div
      class="block"
      v-for="(block, index) in otherBlocks"
      :key="index"
      :style="{ paddingLeft: block.Level != 0 ? block.Level * 20 + 'px' : '40px' }"
    >
      <div
        class="block__level"
        :style="{ backgroundColor: colors[block.Level - 1].color }"
        v-if="block.Level != 0"
      ></div>
      <div v-html="marked(block.Content)" class="markdown-container"></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { DocumentBlock } from '@/entities/Document'
import { marked } from 'marked'
import { onMounted, ref, watch, type PropType } from 'vue'

marked.setOptions({
  breaks: true,
  gfm: true,
})

const colors = [
  {
    color: '#e81c2b',
    name: 'Phần',
  },
  {
    color: '#f4891e',
    name: 'Chương',
  },
  {
    color: '#eace2a',
    name: 'Mục',
  },
  {
    color: '#0aa34f',
    name: 'Tiểu mục',
  },
  {
    color: '#459fe3',
    name: 'Điều',
  },
  {
    color: '#d80b8f',
    name: 'Khoản',
  },
  {
    color: '#6a3499',
    name: 'Điểm',
  },
]

const props = defineProps({
  blocks: {
    type: Array as PropType<DocumentBlock[]>,
    required: true,
  },
})

const firstBlocks = ref<DocumentBlock[]>([])
const contentBlocks = ref<DocumentBlock[]>([])
const signBlocks = ref<DocumentBlock[]>([])
const otherBlocks = ref<DocumentBlock[]>([])

function handleSplitBlock() {
  firstBlocks.value = []
  contentBlocks.value = []
  signBlocks.value = []
  otherBlocks.value = []

  props.blocks.forEach((block) => {
    if (block.ContentType == 1) {
      firstBlocks.value.push(block)
    } else if (block.ContentType == 2) {
      contentBlocks.value.push(block)
    } else if (block.ContentType == 4) {
      signBlocks.value.push(block)
    } else {
      otherBlocks.value.push(block)
    }
  })
}

watch(
  () => props.blocks,
  () => {
    handleSplitBlock()
  },
)

onMounted(() => {
  handleSplitBlock()
})
</script>

<style lang="scss" scoped>
.document-blocks {
  display: flex;
  flex-direction: column;
  .block {
    display: flex;
    flex-direction: row;
    align-items: flex-start;
    gap: 6px;
    border-bottom: 1px solid #d6d6d6;
    border-radius: 0;
    padding: 8px 0;
    padding-right: 20px;
    min-height: 32px;
    cursor: pointer;

    &:hover {
      background-color: #f5f5f5;
    }

    .block__level {
      width: 10px !important;
      height: 10px !important;
      padding: 0 !important;
      border-radius: 50% !important;
      margin-top: 3px;
      flex-shrink: 0;
    }
  }
}
</style>
