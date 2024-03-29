describe("Test team pages", function () {
    it("Create page be unavailable as no user is logged in", function () {
        cy.visit("http://localhost:5289/Teams/Create");
        cy.url().should("include", "Account/Login?ReturnUrl=%2FTeams%2FCreate");
    });
    it("Create page should not be available for team member", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#FirstName").type("Test");
        cy.get("#LastName").type("Test");
        cy.get("#Role").select("Team Member");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("#ConfirmPassword").type("Test-1234");
        cy.get("input[value='Register']").click();
        cy.request({
            url: "http://localhost:5289/Teams/Create",
            failOnStatusCode: false,
            followRedirect: false,
        })
            .its("status")
            .should("equal", 302);
    });
    it("Create page should be available for Manager", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#FirstName").type("Test2");
        cy.get("#LastName").type("Test2");
        cy.get("#Role").select("Manager");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("#ConfirmPassword").type("Test-1234");
        cy.get("input[value='Register']").click();
        cy.visit("http://localhost:5289/Teams/Create");
        cy.url().should("include", "/Teams/Create");
    });
    it("Team Details page should not be available if user hasn't created team", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        // Attempt to visit a team details page
        cy.visit("http://localhost:5289/Teams/Details/1");
        // Verify user is redirected to create team
        cy.url().should("equal", "http://localhost:5289/Teams/Create");
    });
    it("Team Edit page should not be available if user hasn't created team", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        // Attempt to visit a team edit page
        cy.visit("http://localhost:5289/Teams/Edit/1");
        // Verify user is redirected to create team
        cy.url().should("equal", "http://localhost:5289/Teams/Create");
    });
    it("Create page should display validation", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Teams/Create");
        cy.get("input[value='Create']").click();
        cy.get("#TeamName-error")
            .should("be.visible")
            .and("contain", "The Team Name field is required.");
    });
    it("Should create team successfully", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Teams/Create");
        cy.get("#TeamName").type("Test Team");
        cy.get("input[value='Create']").click();
        cy.url().should("include", "/Teams/Details/");
    });
    it("Should display created team in account details", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.get("#TeamName").should("have.value", "4");
    });
    it("New team should be available for new users", function () {
        cy.visit("http://localhost:5289/Account/Register");
        cy.get("#FirstName").type("Test3");
        cy.get("#LastName").type("Test3");
        cy.get("#Role").select("Team Member").trigger("click"); // Ensure on click event is triggered
        cy.get("#TeamId").select("Test Team");
        cy.get("#Email").type("test3@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("#ConfirmPassword").type("Test-1234");
        cy.get("input[value='Register']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.get("#TeamName").should("have.value", "4");
    });
    it("New team should be available for existing users to join", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.get("#updateButton").click();
        cy.get("#TeamName").select("Test Team");
        cy.get("#TeamName").should("have.value", "4");
    });
    it("Manager should be redirected to team they manage on attempt to access different team", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Teams/Details/1");
        cy.url().should("equal", "http://localhost:5289/Teams/Details/4");
    });
    it("Manager should be able to rename team", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Teams/Edit/1");
        cy.url().should("equal", "http://localhost:5289/Teams/Edit/4");
        cy.get("#TeamName").clear();
        cy.get("#TeamName").type("Test Team 2");
        cy.get("input[value='Save']").click();
        cy.url().should("equal", "http://localhost:5289/Teams/Details/4");
        cy.contains("Test Team 2");
    });
    it("Manager should be able to delete team and team members should have team removed from account details", function () {
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test_manager@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Teams/Delete/4");
        cy.get("input[value='Delete']").click();
        cy.url().should("equal", "http://localhost:5289/");
        cy.get(".alert")
            .should("be.visible")
            .and("include.text", "Team has successfully been deleted!");
        cy.visit("http://localhost:5289/Account/Details");
        cy.get("#TeamName").should("have.value", "");
        cy.contains("Delete Account?").click();
        cy.get("button[value='Delete'").click();
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test3@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.get("#TeamName").should("have.value", "");
        cy.contains("Delete Account?").click();
        cy.get("button[value='Delete'").click();
        // Remaining account cleanup
        cy.visit("http://localhost:5289/Account/Login");
        cy.get("#Email").type("test@test.com");
        cy.get("#Password").type("Test-1234");
        cy.get("input[value='Login']").click();
        cy.visit("http://localhost:5289/Account/Details");
        cy.contains("Delete Account?").click();
        cy.get("button[value='Delete'").click();
    });
});
