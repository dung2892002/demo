<template>
  <div
    class="chart-container"
    style="height: auto; margin: 20px 0"
    :style="{ width: `${props.width}px` }"
  >
    <canvas :id="chartId"></canvas>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import Chart, { type ChartData, type ChartOptions, type ChartType } from 'chart.js/auto'

const props = defineProps<{
  chartId: string
  chartType: ChartType
  chartData: ChartData<ChartType>
  chartOptions?: ChartOptions<ChartType>
  width?: number
}>()

const chartInstance = ref<Chart | null>(null)

function createChart() {
  const canvas = document.getElementById(props.chartId) as HTMLCanvasElement
  if (!canvas) {
    return
  }

  const ctx = canvas.getContext('2d')
  if (!ctx) {
    return
  }

  if (chartInstance.value) {
    chartInstance.value.destroy()
  }

  chartInstance.value = new Chart(ctx, {
    type: props.chartType,
    data: props.chartData,
    options: props.chartOptions,
  })
}

watch(() => props.chartData, createChart, { deep: true })

onMounted(createChart)
</script>
