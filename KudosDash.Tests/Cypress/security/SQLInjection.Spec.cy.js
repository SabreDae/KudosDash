describe("SQL injection testing", function () {
    it("Checks for SQL injection vulnerability on Login form", function () {
        cy.visit("http://localhost:5289/Account/Login?query=1 OR 1=1");
        cy.contains("Error").should("not.exist");
    })
    it("Checks for SQL injection vulnerability on Admin page", function () {

    })
})