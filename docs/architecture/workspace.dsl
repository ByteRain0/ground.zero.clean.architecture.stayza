workspace "Name" "Description" {

    !adrs decisions
    !identifiers hierarchical

    model {
        stayza_software_system = softwareSystem "Stayza software system" {

            aspire_dashboard = container "Aspire Dashboard" "Management dashboard used for local development" {
                tags "AspireBase"
            }
            
            telemetry_collector = container "Telemetry Collector" {
                -> aspire_dashboard "Push telemetry"
            }

            stayza_message_broker = container "RabbitMq message broker"{
                tags "Messagebroker"
            }

            stayza_database = container "Database" {
                tags "Database"
            }
            
            stayza_notification_api = container "Notification API" {
                tags "Service"
            }
            
            stayza_web_api = container "Stayza API" {
                tags "Service"
                -> stayza_database
                -> stayza_message_broker
                -> stayza_notification_api
                -> telemetry_collector
                
                group "Utilities" {
                    otel = component "Runtime diagnostic config" {
                        tags UtilitiesComponent
                        -> telemetry_collector
                    }
                    tickerq = component "TickerQ" {
                        tags UtilitiesComponent
                        -> stayza_database
                    }
                    rabbitmq_publisher = component "RabbitMq publisher" {
                        tags UtilitiesComponent
                        -> stayza_message_broker
                    }
                    link_generator = component "Link generator" {
                        tags UtilitiesComponent
                    }
                    claims_identity = component "Claims identity" {
                        tags UtilitiesComponent
                    }
                }
                
                group "Book sub-domain" {
                    book_aggregate_root = component "Book Aggregate Root" {
                        tags "DomainLayerComponent"
                    }
                    book_repository_interface = component "Book repository interface" {
                        tags "DomainLayerComponent"
                        -> book_aggregate_root "Create/Update/Delete operations on AR"
                    }
                    book_repository = component "Book Repository" {
                        tags "InfrastructureLayerComponent"
                        -> book_repository_interface "Implements"
                    }
                    book_service = component "Book service" {
                        -> book_repository_interface
                    }
                    book_endpoints = component "Book endpoints" {
                        tags "ApiLayerComponent"
                        -> book_service
                        -> link_generator
                        -> claims_identity
                    }
                }
                
                group "Loans sub-domain" {
                    loan_entity = component "Loan" {
                        tags "DomainLayerComponent"
                    }
                    reservation_entity = component "Reservation" {
                        tags "DomainLayerComponent"
                    }
                    book_copy_aggregate_root = component "Book copy Aggregate Root" {
                        tags "DomainLayerComponent"
                        -> loan_entity
                        -> reservation_entity
                    }
                    loans_repository_interface = component "Loans repository interface" {
                        tags "DomainLayerComponent"
                        -> book_copy_aggregate_root
                    }
                    loans_repository = component "Loans repository" {
                        tags "InfrastructureLayerComponent"
                        -> loans_repository_interface "Implement"
                    }
                    user_notifications_service_interface = component "IUserNotification service" {
                        tags "DomainLayerComponent"
                    }
                    user_notification_service = component "User Notification service" {
                        tags "InfrastructureLayerComponent"
                        -> user_notifications_service_interface
                        -> stayza_notification_api "Send notifications"
                    }
                    loans_background_jobs = component "Loans background jobs" {
                        -> loans_repository_interface "Use to fetch/update reservations"
                        -> user_notifications_service_interface "Send user alerts"
                    }
                    book_copy_returned_event_handler = component "Book copy returned event handler" {
                        -> stayza_message_broker "Subscribe to events"
                        -> loans_repository_interface
                    }
                    reservation_fufilled_event_handler = component "Reservation fufilled event handler" {
                        -> stayza_message_broker "Subscribe to events"
                        -> user_notifications_service_interface "Notify user"
                    }
                    loans_service = component "Loans service" {
                        -> loans_repository_interface
                    }
                    loans_endpoints = component "Loans endpoints" {
                        tags "ApiLayerComponent"
                        -> loans_service "Uses"
                    }
                }
                
                group "Users sub-domain" {
                    # Omitted for brevity.
                }
                
                group "Persistence setup" {
                    ef_core_db_context = component "EF Core context" {
                        tags "InfrastructureLayerComponent"
                        -> stayza_database "Store and Retrieve data"
                        -> tickerq "Set up"
                    }
                    ef_core_domain_events_interceptor = component "EF Core interceptor"{
                        tags "InfrastructureLayerComponent"
                        -> ef_core_db_context "Intercept SaveChanges()"
                        -> rabbitmq_publisher "Publish domain events"
                    }
                    ef_core_domain_model_configurations = component "Domain model configurations" {
                        tags "InfrastructureLayerComponent"
                        -> ef_core_db_context
                    }
                    ef_core_data_seed = component "Domain data seed" {
                        tags "InfrastructureLayerComponent"
                        -> ef_core_db_context
                    }
                    book_repository -> ef_core_db_context "Store and retrieve data"
                    loans_repository -> ef_core_db_context "Store and retrieve data"
                }
                
                
                
            }
            
            stayza_web_app = container "Stayza Web App" {
                -> stayza_web_api
            }


        }

        user = person "User" {
            -> stayza_software_system.stayza_web_app "Uses"
        }

    }

    views {
        systemContext stayza_software_system "Diagram1" {
            include *
            autolayout lr
        }

        container stayza_software_system "software_system_view" {
            include *
            autolayout lr
        }
        component stayza_software_system.stayza_web_api "api_component_view" {
            include *
            autolayout lr
        }

        styles {
            element "Element" {
                color #ffffff
            }
            element "Person" {
                background #048c04
                shape person
            }
            element "Software System" {
                background #047804
            }
            element "Container" {
                background #55aa55
            }
            element "Component" {
                background #55aa55
            }
            element "UtilitiesComponent"{
                background #444543
            }
            element "DomainLayerComponent" {
                background #ab9a03
            }
            element "ApplicationLayerComponent" {
                background #395703
            }
            element "InfrastructureLayerComponent" {
                background #57031c
            }
            element "ApiLayerComponent"{
                background #046987
            }
            element "AspireBase" {
                background #7242f5
            }
            element "Database" {
                shape cylinder
            }
            element "Messagebroker" {
                shape Pipe
            }
            element "WebBrowser" {
                shape Window
            }
            element "MobileApp" {
                shape MobileDevicePortrait
            }
            element "Service" {
                shape hexagon
            }
            element "FileStorage" {
                shape folder
            }
            element "ExternalActor" {
                background #999999
            }
            element "ExternalSystem" {
                background #999999
            }
            element "ObservabilityBE" {
                background #ebc934
            }
        }
    }

    configuration {
        scope softwaresystem
    }

}