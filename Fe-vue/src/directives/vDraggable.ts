import type { Directive } from 'vue'

const vDraggable: Directive = {
  mounted(el) {
    const header = el.querySelector('.form__header')
    if (!header) {
      console.warn('Không tìm thấy phần header để làm điểm bắt đầu kéo')
      return
    }

    let isDragging = false
    let offset = { x: 0, y: 0 }

    function startDrag(event: MouseEvent) {
      isDragging = true
      const rect = el.getBoundingClientRect()
      offset = {
        x: event.clientX - rect.left,
        y: event.clientY - rect.top,
      }
      document.addEventListener('mousemove', drag)
      document.addEventListener('mouseup', stopDrag)
    }

    function drag(event: MouseEvent) {
      if (isDragging) {
        el.style.position = 'absolute'
        el.style.left = `${event.clientX - offset.x}px`
        el.style.top = `${event.clientY - offset.y}px`
      }
    }

    function stopDrag() {
      isDragging = false
      document.removeEventListener('mousemove', drag)
      document.removeEventListener('mouseup', stopDrag)
    }

    header.style.cursor = 'move'
    header.addEventListener('mousedown', startDrag)
  },
}

export default vDraggable
