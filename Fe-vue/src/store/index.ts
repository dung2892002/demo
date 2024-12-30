import { state } from './state'
import { mutations } from './mutations'
import { actions } from './actions'
import { getters } from './getters'
import type { State } from './types'
import { createStore } from 'vuex'
export const store = createStore<State>({
  state,
  mutations,
  actions,
  getters,
})
