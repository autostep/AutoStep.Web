Feature: General Selection

    Scenario Outline: Can Find Parent of Child

        Given I have navigated to /element in the basic application
        
        Then the <parent> parent element of the '.<container-id> <child>' element exists
        
    Examples: 
        | container-id     | parent  | child  |
        | immediate-parent | .parent | .child |
        | indirect-parent  | .parent | .child | 
        
        
