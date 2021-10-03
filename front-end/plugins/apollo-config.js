import { setContext } from 'apollo-link-context'
import { ApolloLink } from 'apollo-link'
import { onError } from 'apollo-link-error'
import { RetryLink } from 'apollo-link-retry'
import { InMemoryCache } from 'apollo-cache-inmemory'

export default ({ $config, app, store }) => {
  const headers = setContext(() => {
    const headers = {}

    let locale = 'en'
    if (typeof localStorage !== 'undefined') {
      locale = localStorage.getItem('locale')
    }

    if (locale == null) {
      locale = 'en'
    }

    return {
      headers: {
        ...headers,
        'X-Request-Language': locale
      }
    }
  })

  const error = onError(
    ({ operation, graphQLErrors, networkError, forward }) => {
      console.log(graphQLErrors)
      console.log(networkError)
    }
  )

  const retry = new RetryLink({
    delay: {
      initial: 100,
      max: 2000,
      jitter: true
    },
    attempts: {
      max: 3,
      retryIf: (error) => {
        const doNotRetryCodes = [500, 400]
        return !!error && !doNotRetryCodes.includes(error.statusCode)
      }
    }
  })

  const link = ApolloLink.from([error, headers, retry])

  return {
    link,
    httpEndpoint: process.env.GRAPHQL_API,
    cache: new InMemoryCache()
  }
}
