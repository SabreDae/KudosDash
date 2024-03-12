import { describe, it } from "mocha";
import { cy } from "cypress";
describe("JS injection testing", function () {
    it("Checks for JS injection vulnerability on Login Page", function () {
        cy.visit("http://localhost:5289/Account/Login?javascript:alert(1)");
        cy.contains("Error").should("not.exist");
    });
    it("Checks for JS injection vulnerability on Login Page", function () {
        cy.visit(
            "http://localhost:5289/Account/Login#&lt;script&gt;window.location='www.haxxed.com'&lt;/script#&gt"
        );
        cy.contains("Error").should("not.exist");
    });
    it("Checks for JS injection vulnerability in Login form", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get('input[name="Email"]').type("test@test.com");
        cy.get('input[name="Password"]').type('<script>alert("XSS")</script>');
        cy.get("form").submit();
        cy.contains('<script>alert("XSS")</script>').should("not.exist");
    });
    it("Checks for JS injection vulnerability on Registration Page", function () {
        cy.visit("http://localhost:5289/Account/Register?javascript:alert(1)");
        cy.contains("Error").should("not.exist");
    });
    it("Checks for JS injection vulnerability on Registration Page", function () {
        cy.visit(
            "http://localhost:5289/Account/Register#&lt;script&gt;window.location='www.haxxed.com'&lt;/script#&gt"
        );
        cy.contains("Error").should("not.exist");
    });
    it("Checks for JS injection vulnerability in Register form", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#FirstName").type('<script>alert("XSS")</script>');
        cy.get("#LastName").type("Test");
        cy.get("#Role").select(2);
        cy.get("#Email").type("testing@testing.com");
        cy.get("#Password").type("Test-945");
        cy.get("#ConfirmPassword").type("Test-945");
        cy.get("form").submit();
        cy.contains('<script>alert("XSS")</script>').should("not.exist");
        // JS injection is encoded as plain text and account is created with name - clean up created account from the database
        cy.visit("http://localhost:5289/Account/Delete");
        cy.get('button[value="Delete"]').click();
    });
});
