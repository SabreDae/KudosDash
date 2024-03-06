describe("SQL injection testing", function () {
    it("Checks for SQL injection vulnerability on Login Page", function () {
        cy.visit("http://localhost:5289/Account/Login?query=1 OR 1=1");
        cy.contains("Error").should("not.exist");
    })
    it("Checks for SQL injection vulnerability in Login form", function () {
        cy.visit("http://localhost:5289/Account/Login")
        cy.get("#Email").type("' OR 1=1-- ")
        cy.get("input[value='Login']").click()
        cy.get("#Email-error").should("be.visible").and("contain", "Please enter a valid email address.")
    })
    it("Checks for SQL injection vulnerability on Admin page", function () {
        cy.visit("http://localhost:5289/Admin?query=1 OR 1=1");
        cy.contains("Error").should("not.exist");
    })
    it("Checks for SQL injection vulnerability on Registration page", function () {
        cy.visit("http://localhost:5289/Account/Register?query=1 OR 1=1");
        cy.contains("Error").should("not.exist");
    })
})