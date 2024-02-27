const { defineConfig } = require("cypress")

module.exports = defineConfig({
  projectId: 'r9jgmg',
    e2e: {
        baseUrl: "http://localhost:7197",
        specPattern: "**/*.cy.{js,ts}",
        supportFile: "KudosDash.Tests/Cypress/support/index.js",
    },
})