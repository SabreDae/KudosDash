describe("Test user login", function () {
    it("Login page should be up", function () {
        cy.visit("https://localhost:7197/Account/Login")
    })
    it("should show validation errors when no information was entered", function () {
        cy.visit("https://localhost:7197/Account/Login")
        cy.get("input[value='Login']").click()
        // Implement a wait to ensure async processes for errors can run
        cy.get("#Email-error").should("be.visible").and("contain", "Please enter a valid email address.")
        cy.get("#Password-error").should("be.visible").and("contain", "Please enter a password.")
    })
    it("should show validation errors when incorrect email format is entered", function () {
        cy.visit("https://localhost:7197/Account/Login")
        cy.get("#Email").type("test")
        // Implement a wait to ensure async processes for errors can run
        cy.get("#Password").click()
        cy.get("#Email-error").should("be.visible").and("contain", "Please enter a valid email address.")
    })
    //it("should login when correct account details are passed", function () {
    //    cy.visit("https://localhost:7197/Account/Login")
    //    cy.get("#Email").type("test@test.com")
    //    cy.get("#Password").type("Test1234")
    //    cy.get("input[value='Login']").click()
    //    cy.get("#navdrop").should("contain", "test@test.com")
    //})
})

