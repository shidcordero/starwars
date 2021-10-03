/**
 * Decodes the id of a record from db, then gets
 * the reference ID in Integer.
 * @param {String} referenceId - The ID of an object from db.
 * @returns
 */
export const decodeReferenceId = (referenceId) => {
  const buff = Buffer.from(referenceId, 'base64')
  const decoded = buff.toString('ascii')

  // Given the decoded Id, this pattern
  // gets all the number after new line  + i/d/s
  // example:
  // User
  // i12
  // This pattern shall return 12
  const regExPattern = /[^\ndis]*$/
  return parseInt(regExPattern.exec(decoded))
}

export const encodeReferenceId = (model, id) => {
  return window.btoa(`${model}\ni${id}`)
}

export const capitalizeFirstLetter = (text) => {
  return text.charAt(0).toUpperCase() + text.slice(1)
}

export const isNumber = (event, allowDecimal) => {
  let pattern = /\d/

  if (allowDecimal) {
    pattern = /[\d.]/
  }

  if (!pattern.test(event.key)) {
    return event.preventDefault()
  }
}
