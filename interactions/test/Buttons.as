Feature: Buttons

    Scenario Outline: Can Locate Buttons

        Given I have navigated to /button in the basic application

        Then the <name> button should be displayed
        
    Examples:
        | name       |
        | Button Tag |
        | Button Tag Submit |
        | Aria-Label Button |
        | Button Role       |
        | Button Role Aria Label |
        | Input Tag Button |
        | Input Tag Submit Button |
        | Input Tag Reset Button |
        | Aria-Label Button |

    Scenario Outline: Can Detect Button Enabled State

        Given I have navigated to /button in the basic application

        Then the <name> button should be enabled        
        
    Examples:
        | name       |
        | Button Tag |
        | Button Tag Submit |
        | Aria-Label Button |
        | Button Role       |
        | Button Role Aria Label |
        | Input Tag Button |
        | Input Tag Submit Button |
        | Input Tag Reset Button |
        | Aria-Label Button |

    Scenario Outline: Can Click Buttons

        Given I have navigated to /button in the basic application

        Given I have clicked the <name> button

        Then the <name> button should be marked as clicked
        
    Examples:
        | name       |
        | Button Tag |
        | Button Tag Submit |
        | Aria-Label Button |
        | Button Role       |
        | Button Role Aria Label |
        | Input Tag Button |
        | Input Tag Submit Button |
        | Input Tag Reset Button |
        | Aria-Label Button |

    $expectingError: Expecting element at index 0 to be enabled, but the element was disabled.
    Scenario: Cannot click disabled button

        Given I have navigated to /button in the basic application
        
        Given I have clicked the Disabled button