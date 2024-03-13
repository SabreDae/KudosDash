describe("Cross-Site Request Forgery testing", function () {
    it("Checks for CSRF on Login, expecting a bad request response", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Account/Login",
            failOnStatusCode: false,
            form: true,
            body: {
                Email: "john.doe@example.com",
                Password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(400);
        });
    });
    it("Checks for CSRF on Login, with valid login details, expecting a bad request response", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Account/Login",
            failOnStatusCode: false,
            form: true,
            body: {
                Email: "test@test.com",
                Password: "Test-1234",
                RememberMe: false,
            },
        }).then((response) => {
            expect(response.status).to.eq(400);
        });
    });
    it("Checks for CSRF on Register, expecting a bad request response", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Account/Register",
            failOnStatusCode: false,
            form: true,
            body: {
                FirstName: "John",
                LastName: "Doe",
                TeamId: 1,
                Role: "Team Member",
                Email: "john.doe@example.com",
                Password: "MadeUp-92",
                ConfirmPassword: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(400);
        });
    });
    it("Checks for CSRF on account update, expexting a redirect because the request isn't from an authenticated user", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Account/Details",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(302);
        });
    });
    it("Checks for CSRF on account delete, expecting an unsupported request response", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Account/Delete",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(405);
        });
    });
    it("Checks for CSRF on Submit Feedback, expexting a redirect because the request isn't from an authenticated user", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Feedback/Create",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(302);
        });
    });
    it("Checks for CSRF on Feedback Index, expecting an unsupported request response", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Feedback/Index",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(405);
        });
    });
    it("Checks for CSRF on Feedback Details, expecting an unsupported request response", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Feedback/Details/1",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(405);
        });
    });
    it("Checks for CSRF on Feedback Edit, expexting a redirect because the request isn't from an authenticated user", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Feedback/Edit/1",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(302);
        });
    });
    it("Checks for CSRF on Feedback Delete, expexting a redirect because the request isn't from an authenticated user", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Feedback/Delete/1",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(302);
        });
    });
    it("Checks for CSRF on Teams Index, expexting a redirect because the request isn't from an authenticated user", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Teams/Index",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(302);
        });
    });
    it("Checks for CSRF on Create Team, expexting a redirect because the request isn't from an authenticated user", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Teams/Create",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(302);
        });
    });
    it("Checks for CSRF on Team Details, expexting a redirect because the request isn't from an authenticated user", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Teams/Details/1",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(302);
        });
    });
    it("Checks for CSRF on Team Edit, expexting a redirect because the request isn't from an authenticated user", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Teams/Edit/1",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(302);
        });
    });
    it("Checks for CSRF on Team Delete, expexting a redirect because the request isn't from an authenticated user", function () {
        cy.request({
            method: "POST",
            url: "http://localhost:5289/Teams/Delete/1",
            failOnStatusCode: false,
            followRedirect: false,
            form: true,
            body: {
                email: "john.doe@example.com",
                password: "MadeUp-92",
            },
        }).then((response) => {
            expect(response.status).to.eq(302);
        });
    });
});
