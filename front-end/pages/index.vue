<template>
  <div class="section">
    <b-field grouped group-multiline>
      <div class="control is-flex">
        <b-field label="Filter by passenger count">
          <b-input
            v-model.number="filter.passenger"
            type="number"
            @keypress.native="isNumber($event)"
            @keyup.native.enter="onFilterClick"
          ></b-input>
          <b-button
            class="ml-5"
            type="is-primary"
            :disabled="loading"
            :loading="loading"
            @click="onFilterClick"
          >
            Filter
          </b-button>
        </b-field>
      </div>
    </b-field>

    <b-table
      ref="table"
      :data="starships"
      paginated
      :per-page="perPage"
      detailed
      detail-key="name"
      detail-transition="fade"
      :show-detail-icon="true"
      :loading="loading"
      aria-next-label="Next page"
      aria-previous-label="Previous page"
      aria-page-label="Page"
      aria-current-label="Current page"
    >
      <b-table-column v-slot="props" field="name" label="Name" sortable>
        {{ props.row.name }}
      </b-table-column>

      <b-table-column v-slot="props" field="model" label="Model" sortable>
        {{ props.row.model }}
      </b-table-column>

      <b-table-column
        v-slot="props"
        field="manufacturer"
        label="Manufacturer"
        sortable
      >
        {{ props.row.manufacturer }}
      </b-table-column>

      <b-table-column
        v-slot="props"
        field="passengers"
        label="Passengers"
        sortable
      >
        {{ props.row.passengers }}
      </b-table-column>

      <template #detail="props">
        <article class="media">
          <figure class="media-left">
            <p class="image is-64x64">
              <img src="/img/starwars.png" />
            </p>
          </figure>
          <div class="media-content">
            <div class="content">
              <h4 class="mb-2">
                {{ props.row.name }}
              </h4>
              <p class="mb-1">
                <b>Model:</b>
                {{ props.row.model }}
              </p>
              <p class="mb-1">
                <b>Manufacturer:</b>
                {{ props.row.manufacturer }}
              </p>
              <p class="mb-1">
                <b>Starship Class:</b>
                {{ props.row.starshipClass }}
              </p>
              <div class="mb-1">
                <b>Pilots:</b>
                <ul v-if="props.row.pilotList.length > 0" class="mt-0">
                  <li
                    v-for="(pilot, index) in props.row.pilotList"
                    :key="index"
                  >
                    {{ pilot.name }}
                  </li>
                </ul>
                <span v-else>none</span>
              </div>
            </div>
          </div>
        </article>
      </template>
    </b-table>
  </div>
</template>

<script>
import { gql } from 'graphql-tag'
import { isNumber } from '@utils'

const STARSHIPS = gql`
  query STARSHIPS(
    $first: Int
    $after: String
    $last: Int
    $before: String
    $where: StarshipFilterInput
    $order: [StarshipSortInput!]
  ) {
    starships(
      first: $first
      after: $after
      last: $last
      before: $before
      where: $where
      order: $order
    ) {
      nodes {
        id
        name
        model
        manufacturer
        passengers
        starshipClass
        pilotList {
          name
        }
      }
      totalCount
    }
  }
`
export default {
  name: 'HomePage',
  data() {
    return {
      isNumber,
      loading: false,
      starships: [],
      perPage: 10,
      filter: { passenger: null }
    }
  },
  methods: {
    onFilterClick() {
      this.refreshData()
    },
    async refreshData() {
      this.loading = true

      try {
        const { data } = await this.$apollo.query({
          query: STARSHIPS,
          variables: {
            first: 999,
            where: {
              parsedPassengers: {
                gte: this.filter.passenger ? parseInt(this.filter.passenger) : 0
              }
            }
          },
          fetchPolicy: 'cache-first'
        })

        this.starships = data.starships.nodes
      } catch (ex) {
        this.$buefy.toast.open({
          duration: 3000,
          message: ex,
          type: 'is-danger'
        })
      }

      this.loading = false
    }
  }
}
</script>
