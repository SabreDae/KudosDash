import { describe, it } from "mocha";
import { cy } from "cypress";
describe("Test user login", function () {
    it("Login page should be up", function () {
        cy.visit("http://localhost:5289/Account/Login");
    });
    it("should show validation errors when no information was entered", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("input[value='Login']").click();
        // Implement a wait to ensure async processes for errors can run
        cy.get("#Email-error")
            .should("be.visible")
            .and("contain", "Please enter a valid email address.");
        cy.get("#Password-error")
            .should("be.visible")
            .and("contain", "Please enter a password.");
    })
    it("should show validation errors when incorrect email format is entered", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test");
        // Implement a wait to ensure async processes for errors can run
        cy.get("#Password").click();
        cy.get("#Email-error")
            .should("be.visible")
            .and("contain", "Please enter a valid email address.");
    });
});
