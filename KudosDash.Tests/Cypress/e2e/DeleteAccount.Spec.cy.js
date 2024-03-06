describe("Test user account deletion", function () {
    it("should not be available if not logged in", function () {
        cy.request({ url: "http://localhost:5289/Account/Delete", failOnStatusCode: false, followRedirect: false }).its("status").should("equal", 302)
    })
})