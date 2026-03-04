Feature: Get Payment
    
    Scenario: Successfully retrieve an existing payment
        Given a payment exists with id "abc-123"
        When I request the payment with id "abc-123"
        Then the response status should be 200
        And the response should contain the payment details

    Scenario: Payment not found
        Given no payment exists with id "xyz-999"
        When I request the payment with id "xyz-999"
        Then the response status should be 404