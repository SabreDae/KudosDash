describe("Test user account deletion", function () {
    it("should not be available if not logged in", function () {
        cy.request({
            url: "http://localhost:5289/Account/Delete",
            failOnStatusCode: false,
            followRedirect: false,
        })
            .its("status")
            .should("equal", 302);
    });
    it("should be available when logged in", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#FirstName").type("Test");
        cy.get("#LastName").type("Test");
        cy.get("#Role").select(2);
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("#ConfirmPassword").type("Test-1234");
        cy.get("input[value='Register']").click();
        cy.visit("http://localhost:5289/Account/Delete");
    });
    it("should succeed", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Delete");
        cy.get("button[value='Delete'").click();
        // Verify User redirected to home and logged out with correct messaging
        cy.url().should("equal", "http://localhost:5289/");
        cy.get(".alert")
            .should("be.visible")
            .and("include.text", "Account successfully deleted!");
        cy.get("#navdrop").should("not.exist");
    });
});
