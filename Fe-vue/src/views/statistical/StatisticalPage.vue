<template>
  <div class="content">
    <div class="content__header">
      <h1 class="content__title">Trang thống kê</h1>
    </div>
    <div class="content-main">
      <div class="main-container">
        <div class="content--row" style="justify-content: space-between; padding: 0 100px">
          <div class="content--column">
            <ChartComponent
              chartId="genderChart"
              chartType="pie"
              :chartData="genderChartData"
              :width="300"
              :chartOptions="{
                plugins: {
                  title: {
                    display: true,
                    text: 'Tỉ lệ giới tính khách hàng',
                    font: {
                      size: 18,
                    },
                    padding: {
                      top: 10,
                      bottom: 10,
                    },
                  },
                },
              }"
            />
          </div>
          <div class="content--column">
            <ChartComponent
              chartId="dobChart"
              chartType="bar"
              :chartData="dobChartData"
              :width="600"
              :chartOptions="{
                datasets: {
                  bar: {
                    categoryPercentage: 0.7,
                    barPercentage: 0.7,
                  },
                },
                plugins: {
                  title: {
                    display: true,
                    text: 'Số lượng khách hàng theo năm sinh',
                    font: {
                      size: 18,
                    },
                    padding: {
                      top: 10,
                      bottom: 10,
                    },
                  },
                },
              }"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import axios from 'axios'
import { onMounted, ref, computed } from 'vue'
import ChartComponent from '@/components/ChartComponent.vue'

interface CustomerGenderStatistical {
  Total: number
  TotalMale: number
  TotalFemale: number
  TotalUnknown: number
  PercentMale: number
  PercentFemale: number
  PercentUnknown: number
}

const customerGenderStatistical = ref<CustomerGenderStatistical>({
  Total: 0,
  TotalMale: 0,
  TotalFemale: 0,
  TotalUnknown: 0,
  PercentMale: 0,
  PercentFemale: 0,
  PercentUnknown: 0,
})

interface CustomerDobStatistical {
  YearValue: number
  CountValue: number
}

const customerDobStatistical = ref<CustomerDobStatistical[]>([])

const genderChartData = computed(() => ({
  labels: ['Nam', 'Nữ', 'Không rõ'],
  datasets: [
    {
      data: [
        customerGenderStatistical.value.PercentMale,
        customerGenderStatistical.value.PercentFemale,
        customerGenderStatistical.value.PercentUnknown,
      ],
      backgroundColor: ['#4caf50', '#ff9800', '#f44336'],
    },
  ],
}))

const dobChartData = computed(() => ({
  labels: customerDobStatistical.value.map((item) => item.YearValue.toString()),
  datasets: [
    {
      label: 'Số lượng',
      data: customerDobStatistical.value.map((item) => item.CountValue),
      backgroundColor: '#4caf50',
    },
  ],
}))

async function fetchDataCustomerGender() {
  try {
    const response = await axios.get('https://localhost:7160/api/v1/Customers/statistical/gender')
    customerGenderStatistical.value = response.data
  } catch (error) {
    console.log(error)
  }
}

async function fetchDataCustomerDob() {
  try {
    const response = await axios.get('https://localhost:7160/api/v1/Customers/statistical/dob')
    customerDobStatistical.value = response.data
  } catch (error) {
    console.log(error)
  }
}

onMounted(() => {
  fetchDataCustomerGender()
  fetchDataCustomerDob()
})
</script>
