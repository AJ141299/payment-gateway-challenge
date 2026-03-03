Feature: Process Payment

    Scenario: Successfully process an authorized payment
        Given the bank will authorize the payment
        When I submit a valid payment request
        Then the response status should be 200
        And the response should contain an authorized payment

    Scenario: Successfully process a declined payment
        Given the bank will decline the payment
        When I submit a valid payment request
        Then the response status should be 200
        And the response should contain a declined payment