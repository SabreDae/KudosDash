describe("Test user account deletion", function () {
    it("should not be available if not logged in", function () {
        cy.request({ url: "https://localhost:7197/Account/Delete", failOnStatusCode: false}).its("status").should("equal", 404)
    })
})