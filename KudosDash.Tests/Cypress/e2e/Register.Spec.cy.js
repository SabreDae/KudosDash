import { describe, it } from "mocha";
import { cy } from "cypress";
describe("Test User Registration", function () {
    it("Registration page is accessible", function () {
        cy.visit("http://localhost:5289/Account/Register");
    });
    it("should show validation errors when no information was entered", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("input[value='Register']").click();
        // Verify all errors show
        cy.get("#FirstName-error")
            .should("be.visible")
            .and("contain", "The First Name field is required.");
        cy.get("#LastName-error")
            .should("be.visible")
            .and("contain", "The Last Name field is required.");
        cy.get("#Role-error")
            .should("be.visible")
            .and("contain", "The Role field is required.");
        cy.get("#Email-error")
            .should("be.visible")
            .and("contain", "The Email field is required.");
        cy.get("#Password-error")
            .should("be.visible")
            .and("contain", "The Password field is required.");
        cy.get("#ConfirmPassword-error")
            .should("be.visible")
            .and("contain", "The Re-enter password field is required.");
    });
    it("should show email validation error on incorrectly formatted input", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#Email").type("HelloWorld");
        cy.get("#Password").click();
        cy.get("#Email-error")
            .should("be.visible")
            .and("contain", "Please enter a valid email address.");
    });
    it("should show mismatched passwords error", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#Password").type("Test1234");
        cy.get("#ConfirmPassword").type("Test");
        cy.get("#LastName").click();
        cy.get("#ConfirmPassword-error")
            .should("be.visible")
            .and("contain", "Passwords do not match.");
    });
    it("should show errors for password complexity", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#FirstName").type("Alfie");
        cy.get("#LastName").type("Test");
        cy.get("#Role").select(2);
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test");
        cy.get("#ConfirmPassword").type("Test");
        cy.get("input[value='Register']").click();
        cy.get(".validation-summary-errors").should(
            "contain",
            "Passwords must be at least 8 characters."
        );
        cy.get(".validation-summary-errors").should(
            "contain",
            "Passwords must have at least one digit ('0'-'9')."
        );
    });
});
