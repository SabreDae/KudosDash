describe("Test User Registration", function () {
    it("Registration page is accessible", function () {
        cy.visit("https://localhost:7197/Account/Register")
    })

    it("Register with no information doesn't work", function () {
        cy.visit("https://localhost:7197/Account/Register")
        cy.get("input[value='Register']").click()
        // Verify all errors show
        cy.get("#FirstName-error").should("be.visible").and("contain", "The First Name field is required.")
        cy.get("#LastName-error").should("be.visible").and("contain", "The Last Name field is required.")
        cy.get("#Role-error").should("be.visible").and("contain", "The Role field is required.")
        cy.get("#Email-error").should("be.visible").and("contain", "The Email field is required.")
        cy.get("#Password-error").should("be.visible").and("contain", "The Password field is required.")
        cy.get("#ConfirmPassword-error").should("be.visible").and("contain", "The Re-enter password field is required.")
    })

    it("Register email validation works", function () {
        cy.visit("https://localhost:7197/Account/Register")
        cy.get("#Email").type("HelloWorld")
        cy.get("#Password").click()
        cy.get("#Email-error").should("be.visible").and("contain", "Please enter a valid email address.")
    })

    it("Register password comparison causes correct error", function () {
        cy.visit("https://localhost:7197/Account/Register")
        cy.get("#Password").type("Test1234")
        cy.get("#ConfirmPassword").type("Test")
        cy.get("#LastName").click()
        cy.get("#ConfirmPassword-error").should("be.visible").and("contain", "Passwords do not match.")
    })

    it("Register with simple password does not work", function () {
        cy.visit("https://localhost:7197/Account/Register")
        cy.get("#FirstName").type("Alfie")
        cy.get("#LastName").type("Test")
        cy.get("#Role").select(3)
        cy.get("#Email").type("test@test.com")
        cy.get("#Password").type("Test")
        cy.get("#ConfirmPassword").type("Test")
        cy.get("input[value='Register']").click()
        cy.get(".validation-summary-errors").should("contain", "Passwords must be at least 8 characters.")
        cy.get(".validation-summary-errors").should("contain", "Passwords must have at least one digit ('0'-'9').")
    })
    it("Register successful", function () {
        cy.visit("https://localhost:7197/Account/Register")
    })
})