describe("Test user login", function () {
    it("Login page should be up", function () {
        cy.visit("https://localhost:7197/Account/Login")
    })
    it("Login with no details entered does not work", function () {
        cy.visit("https://localhost:7197/Account/Login")
        cy.get("input[value='Login']").click()
        // Implement a wait to ensure async processes for errors can run
        cy.get("#Email-error").should("be.visible").and("contain", "Please enter a valid email address.")
        cy.get("#Password-error").should("be.visible").and("contain", "Please enter a password.")
    })
    it("Login with malformed email does not work", function () {
        cy.visit("https://localhost:7197/Account/Login")
        cy.get("#Email").type("test")
        // Implement a wait to ensure async processes for errors can run
        cy.get("#Password").click()
        cy.get("#Email-error").should("be.visible").and("contain", "Please enter a valid email address.")
    })
})