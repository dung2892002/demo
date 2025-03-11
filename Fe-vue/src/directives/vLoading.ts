import type { Directive } from 'vue'

const vLoading: Directive = {
  mounted: (el, binding) => {
    const loader = document.createElement('div')
    loader.classList.add('loading-overlay')
    loader.style.visibility = binding.value ? 'visible' : 'hidden'
    el.style.position = 'relative'
    el.style.overflow = 'hidden'
    el.appendChild(loader)

    const spin = document.createElement('div')
    spin.classList.add('spin')
    loader.appendChild(spin)

    el._loader = loader
  },
  updated(el, binding) {
    const loader = el._loader
    if (loader) {
      loader.style.visibility = binding.value ? 'visible' : 'hidden'
    }
  },
}

export default vLoading
