workspace "Name" "Description" {

    !adrs decisions
    !identifiers hierarchical

    model {
        u = person "User"
        ss = softwareSystem "Stayza software system" {
            fe = container "Front end app"
            api = container ".NET Api"
            ns = container "Notification System"
            mb = container "RabbitMq message broker"{
                tags "Messagebroker"
            }
            db = container "Database" {
                tags "Database"
            }
        }

        u -> ss.fe "Uses"
        ss.fe -> ss.api "Integrates via HTTP" 
        ss.api -> ss.db "Reads from and writes to"
        ss.api -> ss.ns "Integrates via HTTP"
        ss.api -> ss.mb "Readns and writes to"
    }

    views {
        systemContext ss "Diagram1" {
            include *
            autolayout lr
        }

        container ss "Diagram2" {
            include *
            autolayout lr
        }

        styles {
            element "Element" {
                color #ffffff
            }
            element "Person" {
                background #ba1e75
                shape person
            }
            element "Software System" {
                background #d92389
            }
            element "Container" {
                background #f8289c
            }
            element "Database" {
                shape cylinder
            }
            element "Messagebroker" {
                shape Pipe
            }
        }
    }

    configuration {
        scope softwaresystem
    }

}