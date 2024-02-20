describe("Test Server is running and home page available", function () {
    it("Home page should be up", function () {
        cy.visit("https://localhost:7197")
    })
    it("Link for registration present with redirect", function () {
        cy.visit("https://localhost:7197")
        cy.get("#RegisterNavLink").click()
        cy.url().should("include", "/Account/Register")
    })
    it("Link for Login present with redirect", function () {
        cy.visit("https://localhost:7197")
        cy.contains("Login").click()
        cy.url().should("include", "/Account/Login")
    })
    it("Button for Team Creation works", function () {
        cy.visit("https://localhost:7197")
        cy.contains("Register your Team").click()
        // Verify redirect to Login as no logged in user
        cy.url().should("include", "Account/Login?ReturnUrl=%2FTeams%2FCreate")
    })
    it("Button for Create your Account works", function () {
        cy.visit("https://localhost:7197")
        cy.contains("Create your Account").click()
        cy.url().should("include", "/Account/Register")
    })
})