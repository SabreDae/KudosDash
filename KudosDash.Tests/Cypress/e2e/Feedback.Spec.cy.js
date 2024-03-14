describe("Test feedback pages", function () {
    it("Index should not be available if not logged in", function () {
        cy.request({
            url: "http://localhost:5289/Feedback",
            failOnStatusCode: false,
            followRedirect: false,
        })
            .its("status")
            .should("equal", 302);
    });
    it("Create should not be available if not logged in", function () {
        cy.request({
            url: "http://localhost:5289/Feedback/Create",
            failOnStatusCode: false,
            followRedirect: false,
        })
            .its("status")
            .should("equal", 302);
    });
    it("Index should be available if logged into an account", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#FirstName").type("Test");
        cy.get("#LastName").type("Test");
        cy.get("#Role").select(2);
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("#ConfirmPassword").type("Test-1234");
        cy.get("input[value='Register']").click();
        cy.visit("http://localhost:5289/Feedback/");
    });
    it("Create should not be available if the user doesn't have an active team membership", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Feedback/Create");
        cy.url().should("equal", "http://localhost:5289/Account/Details");
        cy.get(".alert")
            .should("be.visible")
            .and(
                "include.text",
                "Please join a team before trying to submit feedback for colleagues!"
            );
    });
    it("Create should not be available if the relevant team has no other members", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#FirstName").type("Test");
        cy.get("#LastName").type("Test");
        cy.get("#Role").select("Manager");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("#ConfirmPassword").type("Test-1234");
        cy.get("input[value='Register']").click();
        cy.visit("http://localhost:5289/Teams/Create");
        cy.get("#TeamName").type("Test Feedback Team");
        cy.get("input[value='Create']").click();
        cy.visit("http://localhost:5289/Feedback/Create");
        cy.url().should("equal", "http://localhost:5289/");
        cy.get(".alert")
            .should("be.visible")
            .and(
                "include.text",
                "Sorry, it looks like your colleagues haven't joined this team yet."
            );
    });
    it("Create should be available if the relevant team has other members", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.get("#updateButton").click();
        cy.get("#TeamName").select("Test Feedback Team");
        cy.get("#submitButton").click();
        cy.visit("http://localhost:5289/Feedback/Create");
    });
    it("Create should display validation", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Feedback/Create");
        cy.get("input[value='Submit Feedback']").click();
        cy.get("#TargetUser-error")
            .should("be.visible")
            .and("contain", "The User field is required.");
        cy.get("#FeedbackEditor-error")
            .should("be.visible")
            .and("contain", "The Feedback field is required.");
    });
    it("Create should work", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Feedback/Create");
        cy.get("#TargetUser").select(1);
        cy.get("#FeedbackEditor").type("This is a test entry.");
        cy.get("input[value='Submit Feedback']").click();
        cy.url().should("equal", "http://localhost:5289/Feedback");
        cy.contains("This is a test entry.");
    });
    it("Manager approve should make feedback visible to User", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Feedback/");
        cy.contains("This is a test entry.").should("not.exist");
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Feedback");
        cy.contains("Approve").click();
        cy.get("input[value='Approve']").click();
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Feedback/");
        cy.contains("This is a test entry.");
    });
    it("Manager should not be able to approve feedback for user outside team", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.request({
            url: "http://localhost:5289/Feedback/ManagerApprove/2",
            failOnStatusCode: false,
            followRedirect: false,
        })
            .its("status")
            .should("equal", 302);
    });
    it("User who is not author should not be able to edit feedback", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.request({
            url: "http://localhost:5289/Feedback/Edit/4", 
            failOnStatusCode: false,
            followRedirect: false,
        })
            .its("status")
            .should("equal", 302);
        cy.url().should("equal", "http://localhost:5289/Feedback");
    });
    it("Author should be able to edit feedback", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Feedback/Edit/4");
        cy.url().should("equal", "http://localhost:5289/Feedback/Edit/4");
        cy.get("#FeedbackText").type(" This feedback has been edited.");
        cy.get("input[value='Save']").click();
        cy.contains("This feedback has been edited.");
    });
    it("Team member should not be able to access Feedback Details page", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.request({
            url: "http://localhost:5289/Feedback/Details/4",
            failOnStatusCode: false,
            followRedirect: false,
        })
            .its("status")
            .should("equal", 302);
    });
    it("Manager should be able to access Feedback Details page", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Feedback/Details/4");
        cy.get(".form-check-input"); // form-check is unique to the team details page
    });
    it("Manager should not be able to access Feedback Details page if target user/author is not in same team", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.request({
            url: "http://localhost:5289/Feedback/Details/2",
            failOnStatusCode: false,
            followRedirect: false,
        })
            .its("status")
            .should("equal", 302);
    });
    it("Team member should not be able to access Feedback Delete page", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.request({
            url: "http://localhost:5289/Feedback/Delete/4",
            failOnStatusCode: false,
            followRedirect: false,
        })
            .its("status")
            .should("equal", 302);
    });
    it("Details page should be accessible from Manager's dashboard", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.contains("Details").click();
        cy.url().should("equal", "http://localhost:5289/Feedback/Details/4");
    });
    it("Delete page should be accessible from Manager's dashboard", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.contains("Delete").click();
        cy.url().should("equal", "http://localhost:5289/Feedback/Delete/4");
    });
    it("Manager should be able to access Feedback Delete page", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Feedback/Delete/4");
    });
    it("Manager should not be able to delete Feedback for users/authors not in their team", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.request({
            url: "http://localhost:5289/Feedback/Details/2",
            failOnStatusCode: false,
            followRedirect: false,
        })
            .its("status")
            .should("equal", 302);
    });
    it("Delete request should be successful", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Feedback/Delete/4");
        cy.get("input[value='Delete']").click();
        cy.url().should("equal", "http://localhost:5289/Feedback");
        cy.contains("This is a test entry.").should("not.exist");
        // Cleanup team and accounts
        cy.visit("http://localhost:5289/Teams/Delete/3");
        cy.get("input[value='Delete']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.contains("Delete Account?").click();
        cy.get("button[value='Delete'").click();
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.contains("Delete Account?").click();
        cy.get("button[value='Delete'").click();
    });
});
