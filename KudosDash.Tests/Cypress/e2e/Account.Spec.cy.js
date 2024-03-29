describe("Test user account details", function () {
    it("should not be available if not logged in", function () {
        cy.request({
            url: "http://localhost:5289/Account/Details",
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
        cy.visit("http://localhost:5289/Account/Details");
    });
    it("should have disabled inputs by default", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.get("#submitButton").should("not.be.visible");
        cy.get("#FirstName").should("be.disabled");
        cy.get("#LastName").should("be.disabled");
        cy.get("#Email").should("be.disabled");
        cy.get("#TeamName").should("be.disabled");
    });
    it("should have inputs enabled on button click", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.get("#updateButton").click();
        cy.get("#submitButton").should("be.visible");
        cy.get("#FirstName").should("not.be.disabled");
        cy.get("#LastName").should("not.be.disabled");
        cy.get("#Email").should("not.be.disabled");
        cy.get("#TeamName").should("not.be.disabled");
    });
    it("should have validation on update requests", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.get("#updateButton").click();
        cy.get("#FirstName").clear();
        cy.get("#LastName").clear();
        cy.get("#Email").clear();
        cy.get("#Email").type("test");
        cy.get("#TeamName").select(1);
        cy.get("#submitButton").click();
        cy.get("#FirstName-error")
            .should("be.visible")
            .and("contain", "The First Name field is required.");
        cy.get("#LastName-error")
            .should("be.visible")
            .and("contain", "The Last Name field is required.");
        cy.get("#Email-error")
            .should("be.visible")
            .and("contain", "Please enter a valid email address.");
    });
    it("should have update values on request", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.get("#updateButton").click();
        cy.get("#FirstName").clear();
        cy.get("#LastName").clear();
        cy.get("#Email").clear();
        cy.get("#FirstName").type("Malcolm");
        cy.get("#LastName").type("Middle");
        cy.get("#Email").type("test2@test.com");
        cy.get("#TeamName").select(1);
        cy.get("#submitButton").click();
        cy.get("#FirstName").should("be.disabled").and("have.value", "Malcolm");
        cy.get("#LastName").should("be.disabled").and("have.value", "Middle");
        cy.get("#Email")
            .should("be.disabled")
            .and("have.value", "test2@test.com");
        // Cleanup and delete the test account
        cy.contains("Delete Account?").click();
        cy.get("button[value='Delete'").click();
    });
});
