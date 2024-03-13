describe("Test session timeout", function () {
    it("Should be auto-logged out after 3 minutes", function () {
        cy.clock();
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#FirstName").type("Test");
        cy.get("#LastName").type("Test");
        cy.get("#Role").select(2);
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("#ConfirmPassword").type("Test-1234");
        cy.get("input[value='Register']").click();
        // Session should time out after 3 minutes aka 180000 milliseconds
        cy.wait(180000);
        cy.contains("Submit Feedback").click();
        cy.url().should("include", "/Account/Login");
        // Cleanup by deleting test account
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Delete");
        cy.get("button[value='Delete'").click();
    });
});
