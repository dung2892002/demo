<template>
  <input
    type="text"
    class="form__input"
    v-model="displayDate"
    placeholder="dd/MM/yyyy"
    ref="dateInputRef"
    :class="{ 'disabled': state }"
    :disabled="state"
    @input="handleInput"
    @blur="validateDate"
  />
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue';
import flatpickr from 'flatpickr';
import 'flatpickr/dist/flatpickr.min.css';
import type { Instance } from 'flatpickr/dist/types/instance';

// Định nghĩa props với TypeScript
interface Props {
  modelValue: string;
  state: boolean // Chuỗi ISO 8601 (e.g., "2025-04-17T00:00:00Z")
}
const props = withDefaults(defineProps<Props>(), {
  modelValue: ''
});

// Định nghĩa emits
const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void;
}>();

// Refs
const displayDate = ref<string>('');
const dateInputRef = ref<HTMLInputElement | null>(null);
let flatpickrInstance: Instance | null = null;

// Computed: Chuyển đổi giữa chuỗi ISO 8601 và Date
const internalDate = computed({
  get: () => (props.modelValue ? new Date(props.modelValue) : null),
  set: (newDate: Date | null) => {
    if (newDate && !isNaN(newDate.getTime())) {
      emit('update:modelValue', newDate.toISOString());
    }
  }
});

// Hàm định dạng ngày thành dd/MM/yyyy
const formatDate = (date: Date | null): string => {
  if (!date || isNaN(date.getTime())) return '';
  const dd = String(date.getDate()).padStart(2, '0');
  const mm = String(date.getMonth() + 1).padStart(2, '0');
  const yyyy = date.getFullYear();
  return `${dd}/${mm}/${yyyy}`;
};

// Hàm phân tích chuỗi dd/MM/yyyy thành Date
const parseDate = (str: string): Date | null => {
  const regex = /^(\d{2})\/(\d{2})\/(\d{4})$/;
  if (!regex.test(str)) return null;
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [_, day, month, year] = str.match(regex)!;
  const date = new Date(`${year}-${month}-${day}`);
  if (isNaN(date.getTime()) || date.getDate() != +day || date.getMonth() + 1 != +month) {
    return null;
  }
  return date;
};

// Xử lý khi nhập tay
const handleInput = (event: Event) => {
  const input = event.target as HTMLInputElement;
  let value = input.value.replace(/[^0-9/]/g, '');
  if (value.length === 2 || value.length === 5) {
    value += '/';
  }
  displayDate.value = value.slice(0, 10);
};

// Kiểm tra và xác nhận ngày hợp lệ
const validateDate = () => {
  const parsedDate = parseDate(displayDate.value);
  if (!parsedDate) {
    alert('Vui lòng nhập đúng định dạng dd/MM/yyyy hoặc ngày hợp lệ');
    displayDate.value = formatDate(internalDate.value);
    return;
  }
  internalDate.value = parsedDate;
};

// Khởi tạo Flatpickr
const initializeFlatpickr = () => {
  if (dateInputRef.value) {
    flatpickrInstance = flatpickr(dateInputRef.value, {
      dateFormat: 'd/m/Y',
      allowInput: true,
      defaultDate: internalDate.value ?? undefined,
      onChange: (selectedDates: Date[]) => {
        if (selectedDates.length > 0) {
          internalDate.value = selectedDates[0];
          displayDate.value = formatDate(selectedDates[0]);
        }
      }
    });
  }
};

// Đồng bộ giá trị khi modelValue thay đổi
watch(
  () => props.modelValue,
  (newValue) => {
    const date = newValue ? new Date(newValue) : null;
    displayDate.value = formatDate(date);
    if (flatpickrInstance) {
      flatpickrInstance.setDate(date!);
    }
  }
);

// Khởi tạo và hủy Flatpickr
onMounted(() => {
  displayDate.value = formatDate(internalDate.value);
  initializeFlatpickr();
});

onUnmounted(() => {
  if (flatpickrInstance) {
    flatpickrInstance.destroy();
  }
});
</script>

