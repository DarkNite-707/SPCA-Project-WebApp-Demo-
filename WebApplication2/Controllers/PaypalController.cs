﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System.Threading.Tasks;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace WebApplication2.Controllers
{
    public class PaypalController : Controller
    {

       

        private String _paypalEnvironment = "sandbox";//live
        private String _clientId = "AT3vFguCv6bIn87Qi7mtvgkBPlz1dMajr69yO2RttoCfRogy1Mo6g8ufjed_IAqSKKUwXHc5tACq0OFr";
        private String _secret = "EOgrR1FPdZ0Q3NQMtnHIhcybKD5V3AmJBFOh3PsVMCK-GYkrRTIDbJD3CNp2n3D0erJ2-ZpklWWFXcD7";

        // GET: Paypal
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index_V1()
        {
            return View();
        }

        public ActionResult Index_V2()
        {
            return View();
        }


        public ActionResult Donation()
        {
            return View();
        }

        // This Method was Adapted from Paypal
        //https://developer.paypal.com/docs/checkout/standard/integrate/
        public async Task<ActionResult> Paypalvtwo(string Cancel = null)
        {
            #region local_variables

            //setup paypal environment to save some essential varaibles
            MyPaypalPayment.MyPaypalSetup payPalSetup = new MyPaypalPayment.MyPaypalSetup { Environment = _paypalEnvironment, ClientId = _clientId, Secret = _secret };

            //a list if string to collect messages to be displayed to the payer
            List<string> paymentResultList = new List<string>();

            #endregion

            #region check_payment_cancellation

            //check if payer has cancelled the transaction, if yes, do nothing. Let the payer know about his actions
            if (!string.IsNullOrEmpty(Cancel) && Cancel.Trim().ToLower() == "true")
            {
                paymentResultList.Add("You cancelled the transaction.");
                return View("Index", paymentResultList);
            }

            #endregion

            payPalSetup.PayerApprovedOrderId = Request["token"];
            string PayerID = Request["PayerID"];

            //when payerID is null it means order is not approved by the payer.            
            if (string.IsNullOrEmpty(PayerID))
            {
                #region order_creation
                //Create order and display it to the payer to approve. 
                //This is the first PayPal screen where payer signin using his PayPal credentials

                try
                {
                    //redirect URL. when approved or cancelled on PayPal, PayPal uses this URL to redirect to your app/website.
                    payPalSetup.RedirectUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/Paypal/Paypalvtwo?";
                    HttpResponse response = await MyPaypalPayment.createOrder(payPalSetup);

                    var statusCode = response.StatusCode;
                    Order result = response.Result<Order>();
                    Console.WriteLine("Status: {0}", result.Status);
                    Console.WriteLine("Order Id: {0}", result.Id);
                    Console.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
                    Console.WriteLine("Links:");
                    foreach (PayPalCheckoutSdk.Orders.LinkDescription link in result.Links)
                    {
                        Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
                        if (link.Rel.Trim().ToLower() == "approve")
                        {
                            payPalSetup.ApproveUrl = link.Href;
                        }
                    }

                    if (!string.IsNullOrEmpty(payPalSetup.ApproveUrl))
                        return Redirect(payPalSetup.ApproveUrl);
                }
                catch (Exception ex)
                {
                    paymentResultList.Add("There was an error in processing your payment");
                    paymentResultList.Add("Details: " + ex.Message);
                }

                #endregion
            }
            else
            {
                #region order_execution

                //this is where actual transaction is carried out
                HttpResponse response = await MyPaypalPayment.captureOrder(payPalSetup);
                try
                {
                    var statusCode = response.StatusCode;
                    Order result = response.Result<Order>();
                    Console.WriteLine("Status: {0}", result.Status);
                    Console.WriteLine("Capture Id: {0}", result.Id);

                    //update view bag so user/payer gets to know the status
                    if (result.Status.Trim().ToUpper() == "COMPLETED")
                        paymentResultList.Add("Payment Successful. Thank you.");
                    paymentResultList.Add("Payment ID: " + result.Id);
                    paymentResultList.Add("Payment State: " + result.Status);
                    paymentResultList.Add("Payment Amount: " + result.CheckoutPaymentIntent);

                    #region null_checks
                    if (result.PurchaseUnits != null && result.PurchaseUnits.Count > 0 &&
                        result.PurchaseUnits[0].Payments != null && result.PurchaseUnits[0].Payments.Captures != null &&
                        result.PurchaseUnits[0].Payments.Captures.Count > 0)
                        #endregion
                        paymentResultList.Add("Transaction ID: " + result.PurchaseUnits[0].Payments.Captures[0].Id);
                }
                catch (Exception ex)
                {
                    paymentResultList.Add("There was an error in processing your payment");
                    paymentResultList.Add("Details: " + ex.Message);
                }

                #endregion

            }
            return View("Index", paymentResultList);
        }

        // This Method was Adapted from Paypal
        //https://developer.paypal.com/docs/checkout/standard/integrate/
        public async Task<ActionResult> Paypalvtwo_v1(string Cancel = null)
        {
            #region local_variables

            //setup paypal environment to save some essential varaibles
            MyPaypalPayment_V1.MyPaypalSetup payPalSetup = new MyPaypalPayment_V1.MyPaypalSetup { Environment = _paypalEnvironment, ClientId = _clientId, Secret = _secret };

            //a list if string to collect messages to be displayed to the payer
            List<string> paymentResultList = new List<string>();

            #endregion

            #region check_payment_cancellation

            //check if payer has cancelled the transaction, if yes, do nothing. Let the payer know about his actions
            if (!string.IsNullOrEmpty(Cancel) && Cancel.Trim().ToLower() == "true")
            {
                paymentResultList.Add("You cancelled the transaction.");
                return View("Index", paymentResultList);
            }

            #endregion

            payPalSetup.PayerApprovedOrderId = Request["token"];
            string PayerID = Request["PayerID"];

            //when payerID is null it means order is not approved by the payer.            
            if (string.IsNullOrEmpty(PayerID))
            {
                #region order_creation
                //Create order and display it to the payer to approve. 
                //This is the first PayPal screen where payer signin using his PayPal credentials

                try
                {
                    //redirect URL. when approved or cancelled on PayPal, PayPal uses this URL to redirect to your app/website.
                    payPalSetup.RedirectUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/Paypal/Paypalvtwo?";
                    HttpResponse response = await MyPaypalPayment_V1.createOrder(payPalSetup);

                    var statusCode = response.StatusCode;
                    Order result = response.Result<Order>();
                    Console.WriteLine("Status: {0}", result.Status);
                    Console.WriteLine("Order Id: {0}", result.Id);
                    Console.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
                    Console.WriteLine("Links:");
                    foreach (PayPalCheckoutSdk.Orders.LinkDescription link in result.Links)
                    {
                        Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
                        if (link.Rel.Trim().ToLower() == "approve")
                        {
                            payPalSetup.ApproveUrl = link.Href;
                        }
                    }

                    if (!string.IsNullOrEmpty(payPalSetup.ApproveUrl))
                        return Redirect(payPalSetup.ApproveUrl);
                }
                catch (Exception ex)
                {
                    paymentResultList.Add("There was an error in processing your payment");
                    paymentResultList.Add("Details: " + ex.Message);
                }

                #endregion
            }
            else
            {
                #region order_execution

                //this is where actual transaction is carried out
                HttpResponse response = await MyPaypalPayment_V1.captureOrder(payPalSetup);
                try
                {
                    var statusCode = response.StatusCode;
                    Order result = response.Result<Order>();
                    Console.WriteLine("Status: {0}", result.Status);
                    Console.WriteLine("Capture Id: {0}", result.Id);

                    //update view bag so user/payer gets to know the status
                    if (result.Status.Trim().ToUpper() == "COMPLETED")
                        paymentResultList.Add("Payment Successful. Thank you.");
                    paymentResultList.Add("Payment ID: " + result.Id);
                    paymentResultList.Add("Payment State: " + result.Status);
                    paymentResultList.Add("Payment Amount: " + result.CheckoutPaymentIntent);

                    #region null_checks
                    if (result.PurchaseUnits != null && result.PurchaseUnits.Count > 0 &&
                        result.PurchaseUnits[0].Payments != null && result.PurchaseUnits[0].Payments.Captures != null &&
                        result.PurchaseUnits[0].Payments.Captures.Count > 0)
                        #endregion
                        paymentResultList.Add("Transaction ID: " + result.PurchaseUnits[0].Payments.Captures[0].Id);
                }
                catch (Exception ex)
                {
                    paymentResultList.Add("There was an error in processing your payment");
                    paymentResultList.Add("Details: " + ex.Message);
                }

                #endregion

            }
            return View("Index", paymentResultList);
        }


        // This Method was Adapted from Paypal
        //https://developer.paypal.com/docs/checkout/standard/integrate/
        public async Task<ActionResult> Paypalvtwo_V2(string Cancel = null)
        {
            #region local_variables

            //setup paypal environment to save some essential varaibles
            MyPaypalPayment_V2.MyPaypalSetup payPalSetup = new MyPaypalPayment_V2.MyPaypalSetup { Environment = _paypalEnvironment, ClientId = _clientId, Secret = _secret };

            //a list if string to collect messages to be displayed to the payer
            List<string> paymentResultList = new List<string>();

            #endregion

            #region check_payment_cancellation

            //check if payer has cancelled the transaction, if yes, do nothing. Let the payer know about his actions
            if (!string.IsNullOrEmpty(Cancel) && Cancel.Trim().ToLower() == "true")
            {
                paymentResultList.Add("You cancelled the transaction.");
                return View("Index", paymentResultList);
            }

            #endregion

            payPalSetup.PayerApprovedOrderId = Request["token"];
            string PayerID = Request["PayerID"];

            //when payerID is null it means order is not approved by the payer.            
            if (string.IsNullOrEmpty(PayerID))
            {
                #region order_creation
                //Create order and display it to the payer to approve. 
                //This is the first PayPal screen where payer signin using his PayPal credentials

                try
                {
                    //redirect URL. when approved or cancelled on PayPal, PayPal uses this URL to redirect to your app/website.
                    payPalSetup.RedirectUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/Paypal/Paypalvtwo?";
                    HttpResponse response = await MyPaypalPayment_V2.createOrder(payPalSetup);

                    var statusCode = response.StatusCode;
                    Order result = response.Result<Order>();
                    Console.WriteLine("Status: {0}", result.Status);
                    Console.WriteLine("Order Id: {0}", result.Id);
                    Console.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
                    Console.WriteLine("Links:");
                    foreach (PayPalCheckoutSdk.Orders.LinkDescription link in result.Links)
                    {
                        Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
                        if (link.Rel.Trim().ToLower() == "approve")
                        {
                            payPalSetup.ApproveUrl = link.Href;
                        }
                    }

                    if (!string.IsNullOrEmpty(payPalSetup.ApproveUrl))
                        return Redirect(payPalSetup.ApproveUrl);
                }
                catch (Exception ex)
                {
                    paymentResultList.Add("There was an error in processing your payment");
                    paymentResultList.Add("Details: " + ex.Message);
                }

                #endregion
            }
            else
            {
                #region order_execution

                //this is where actual transaction is carried out
                HttpResponse response = await MyPaypalPayment_V2.captureOrder(payPalSetup);
                try
                {
                    var statusCode = response.StatusCode;
                    Order result = response.Result<Order>();
                    Console.WriteLine("Status: {0}", result.Status);
                    Console.WriteLine("Capture Id: {0}", result.Id);

                    //update view bag so user/payer gets to know the status
                    if (result.Status.Trim().ToUpper() == "COMPLETED")
                        paymentResultList.Add("Payment Successful. Thank you.");
                    paymentResultList.Add("Payment ID: " + result.Id);
                    paymentResultList.Add("Payment State: " + result.Status);
                    paymentResultList.Add("Payment Amount: " + result.CheckoutPaymentIntent);

                    #region null_checks
                    if (result.PurchaseUnits != null && result.PurchaseUnits.Count > 0 &&
                        result.PurchaseUnits[0].Payments != null && result.PurchaseUnits[0].Payments.Captures != null &&
                        result.PurchaseUnits[0].Payments.Captures.Count > 0)
                        #endregion
                        paymentResultList.Add("Transaction ID: " + result.PurchaseUnits[0].Payments.Captures[0].Id);
                }
                catch (Exception ex)
                {
                    paymentResultList.Add("There was an error in processing your payment");
                    paymentResultList.Add("Details: " + ex.Message);
                }

                #endregion

            }
            return View("Index", paymentResultList);
        }


        // This Class was Adapted from Paypal
        //https://developer.paypal.com/docs/checkout/standard/integrate/
        public class MyPaypalPayment
        {
            /// <summary>
            /// Initiates Paypal client. Must ensure correct environment.
            /// </summary>
            /// <param name="paypalEnvironment">provide value sandbox for testing,  provide value live for live environments</param>            
            /// <returns>PayPalHttp.HttpClient</returns>
            public static PayPalHttpClient client(MyPaypalSetup paypalEnvironment)
            {
                PayPalEnvironment environment = null;

                if (paypalEnvironment.Environment == "live")
                {
                    // Creating a live environment
                    environment = new LiveEnvironment(paypalEnvironment.ClientId, paypalEnvironment.Secret);
                }
                else
                {
                    // Creating a sandbox environment
                    environment = new SandboxEnvironment(paypalEnvironment.ClientId, paypalEnvironment.Secret);
                }

                // Creating a client for the environment
                PayPalHttpClient client = new PayPalHttpClient(environment);
                return client;
            }


            //### Creating an Order
            //This will create an order and print order id for the created order

            public async static Task<HttpResponse> createOrder(MyPaypalSetup paypalSetup)
            {
                HttpResponse response = null;

                try
                {
                    // Construct a request object and set desired parameters
                    // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
                    #region order_creation
                    var order = new OrderRequest()
                    {
                        CheckoutPaymentIntent = "CAPTURE",
                        PurchaseUnits = new List<PurchaseUnitRequest>()
                        {
                            new PurchaseUnitRequest()
                            {
                                Items = new List<Item>()
                                {
                                    new Item()
                                    {
                                        Quantity = "1",
                                        Name = "Handicapped Animal Fund",
                                        Description = "Fund to enhance Disabled Animal care",
                                        Sku = "sku",
                                        Tax = new PayPalCheckoutSdk.Orders.Money(){ CurrencyCode = "USD", Value = "0.15" },
                                        UnitAmount = new PayPalCheckoutSdk.Orders.Money(){ CurrencyCode = "USD", Value = "7.99" }
                                    },
                                   
                                },

                                AmountWithBreakdown = new AmountWithBreakdown()
                                {
                                    CurrencyCode = "USD",
                                    Value = "8.14",

                                    AmountBreakdown = new AmountBreakdown()
                                    {
                                        TaxTotal = new PayPalCheckoutSdk.Orders.Money()
                                        {
                                            CurrencyCode = "USD",
                                            Value = "0.15"
                                        },
                                        ItemTotal = new PayPalCheckoutSdk.Orders.Money()
                                        {
                                            CurrencyCode = "USD",
                                            Value = "7.99"
                                        }
                                    }
                                }
                            }
                        },
                        ApplicationContext = new ApplicationContext()
                        {
                            ReturnUrl = paypalSetup.RedirectUrl,
                            CancelUrl = paypalSetup.RedirectUrl + "&Cancel=true"
                        }
                    };

                    #endregion

                    //IMPORTANT
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                    // Call API with your client and get a response for your call
                    var request = new OrdersCreateRequest();
                    request.Prefer("return=representation");
                    request.RequestBody(order);
                    PayPalHttpClient paypalHttpClient = client(paypalSetup);
                    response = await paypalHttpClient.Execute(request);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                    throw;
                }
                return response;
            }


            //### Capturing an Order
            //Before capturing an order, order should be approved by the buyer using the approve link in create order response

            // This Method was Adapted from Paypal
            //https://developer.paypal.com/docs/checkout/standard/integrate/
            public async static Task<HttpResponse> captureOrder(MyPaypalSetup paypalSetup)
            {
                // Construct a request object and set desired parameters
                // Replace ORDER-ID with the approved order id from create order
                var request = new OrdersCaptureRequest(paypalSetup.PayerApprovedOrderId);
                request.RequestBody(new OrderActionRequest());
                PayPalHttpClient paypalHttpClient = client(paypalSetup);
                HttpResponse response = await paypalHttpClient.Execute(request);
                return response;
            }

            // This Class was Adapted from Paypal
            //https://developer.paypal.com/docs/checkout/standard/integrate/
            public class MyPaypalSetup
            {
                /// <summary>
                /// Provide value sandbox for testing,  provide value live for real money
                /// </summary>
                public String Environment { get; set; }
                /// <summary>
                /// Client id as provided by Paypal on dashboard. Ensure you use correct value based on your environment selection
                /// Use sandbox accounts for sandbox testing
                /// </summary>
                public String ClientId { get; set; }
                /// <summary>
                /// Secret as provided by Paypal on dashboard. Ensure you use correct value based on your environment selection
                /// Use sandbox accounts for sandbox testing
                /// </summary>
                public String Secret { get; set; }

                /// <summary>
                /// This is the URL that you will pass to paypal which paypal will use to redirect payer back to your website.
                /// So essentially it is the same controller URL that you must pass
                /// </summary>
                public String RedirectUrl { get; set; }

                /// <summary>
                /// Once order is created on Paypal, it redirects control to your app with a URL that shows order details. Your website must take the payer to this page
                /// so the payer approved the payment. Store this URL in this property
                /// </summary>
                public String ApproveUrl { get; set; }

                /// <summary>
                /// When paypal redirects control to your website it provides a Approved Order ID which we then pass it back to paypal to execute the order.
                /// Store this approved order ID in this property
                /// </summary>
                public String PayerApprovedOrderId { get; set; }
            }

        }



        /// Index_V2
        // This Class was Adapted from Paypal
        //https://developer.paypal.com/docs/checkout/standard/integrate/
        public class MyPaypalPayment_V1
        {
            /// <summary>
            /// Initiates Paypal client. Must ensure correct environment.
            /// </summary>
            /// <param name="paypalEnvironment">provide value sandbox for testing,  provide value live for live environments</param>            
            /// <returns>PayPalHttp.HttpClient</returns>
            public static PayPalHttpClient client(MyPaypalSetup paypalEnvironment)
            {
                PayPalEnvironment environment = null;

                if (paypalEnvironment.Environment == "live")
                {
                    // Creating a live environment
                    environment = new LiveEnvironment(paypalEnvironment.ClientId, paypalEnvironment.Secret);
                }
                else
                {
                    // Creating a sandbox environment
                    environment = new SandboxEnvironment(paypalEnvironment.ClientId, paypalEnvironment.Secret);
                }

                // Creating a client for the environment
                PayPalHttpClient client = new PayPalHttpClient(environment);
                return client;
            }


            //### Creating an Order
            //This will create an order and print order id for the created order

            public async static Task<HttpResponse> createOrder(MyPaypalSetup paypalSetup)
            {
                HttpResponse response = null;

                try
                {
                    // Construct a request object and set desired parameters
                    // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
                    #region order_creation
                    var order = new OrderRequest()
                    {
                        CheckoutPaymentIntent = "CAPTURE",
                        PurchaseUnits = new List<PurchaseUnitRequest>()
                        {
                            new PurchaseUnitRequest()
                            {
                                Items = new List<Item>()
                                {
                                    new Item()
                                    {
                                        Quantity = "1",
                                        Name = "Animal Medication Fund- Donation",
                                        Description = "Funding Medication for The animals",
                                        Sku = "sku",
                                        Tax = new PayPalCheckoutSdk.Orders.Money(){ CurrencyCode = "USD", Value = "0.15" },
                                        UnitAmount = new PayPalCheckoutSdk.Orders.Money(){ CurrencyCode = "USD", Value = "100.99" }
                                    },
                              
                                },

                                AmountWithBreakdown = new AmountWithBreakdown()
                                {
                                    CurrencyCode = "USD",
                                    Value = "101.14",

                                    AmountBreakdown = new AmountBreakdown()
                                    {
                                        TaxTotal = new PayPalCheckoutSdk.Orders.Money()
                                        {
                                            CurrencyCode = "USD",
                                            Value = "0.15"
                                        },                        
                                        ItemTotal = new PayPalCheckoutSdk.Orders.Money()
                                        {
                                            CurrencyCode = "USD",
                                            Value = "100.99"
                                        }
                                    }
                                }
                            }
                        },
                        ApplicationContext = new ApplicationContext()
                        {
                            ReturnUrl = paypalSetup.RedirectUrl,
                            CancelUrl = paypalSetup.RedirectUrl + "&Cancel=true"
                        }
                    };

                    #endregion

                    //IMPORTANT
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                    // Call API with your client and get a response for your call
                    var request = new OrdersCreateRequest();
                    request.Prefer("return=representation");
                    request.RequestBody(order);
                    PayPalHttpClient paypalHttpClient = client(paypalSetup);
                    response = await paypalHttpClient.Execute(request);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                    throw;
                }
                return response;
            }


            //### Capturing an Order
            //Before capturing an order, order should be approved by the buyer using the approve link in create order response

            public async static Task<HttpResponse> captureOrder(MyPaypalSetup paypalSetup)
            {
                // Construct a request object and set desired parameters
                // Replace ORDER-ID with the approved order id from create order
                var request = new OrdersCaptureRequest(paypalSetup.PayerApprovedOrderId);
                request.RequestBody(new OrderActionRequest());
                PayPalHttpClient paypalHttpClient = client(paypalSetup);
                HttpResponse response = await paypalHttpClient.Execute(request);
                return response;
            }

            public class MyPaypalSetup
            {
                /// <summary>
                /// Provide value sandbox for testing,  provide value live for real money
                /// </summary>
                public String Environment { get; set; }
                /// <summary>
                /// Client id as provided by Paypal on dashboard. Ensure you use correct value based on your environment selection
                /// Use sandbox accounts for sandbox testing
                /// </summary>
                public String ClientId { get; set; }
                /// <summary>
                /// Secret as provided by Paypal on dashboard. Ensure you use correct value based on your environment selection
                /// Use sandbox accounts for sandbox testing
                /// </summary>
                public String Secret { get; set; }

                /// <summary>
                /// This is the URL that you will pass to paypal which paypal will use to redirect payer back to your website.
                /// So essentially it is the same controller URL that you must pass
                /// </summary>
                public String RedirectUrl { get; set; }

                /// <summary>
                /// Once order is created on Paypal, it redirects control to your app with a URL that shows order details. Your website must take the payer to this page
                /// so the payer approved the payment. Store this URL in this property
                /// </summary>
                public String ApproveUrl { get; set; }

                /// <summary>
                /// When paypal redirects control to your website it provides a Approved Order ID which we then pass it back to paypal to execute the order.
                /// Store this approved order ID in this property
                /// </summary>
                public String PayerApprovedOrderId { get; set; }
            }

        }

        /// Index_V1
         // This Class was Adapted from Paypal
        //https://developer.paypal.com/docs/checkout/standard/integrate/
        public class MyPaypalPayment_V2
        {
            /// <summary>
            /// Initiates Paypal client. Must ensure correct environment.
            /// </summary>
            /// <param name="paypalEnvironment">provide value sandbox for testing,  provide value live for live environments</param>            
            /// <returns>PayPalHttp.HttpClient</returns>
            public static PayPalHttpClient client(MyPaypalSetup paypalEnvironment)
            {
                PayPalEnvironment environment = null;

                if (paypalEnvironment.Environment == "live")
                {
                    // Creating a live environment
                    environment = new LiveEnvironment(paypalEnvironment.ClientId, paypalEnvironment.Secret);
                }
                else
                {
                    // Creating a sandbox environment
                    environment = new SandboxEnvironment(paypalEnvironment.ClientId, paypalEnvironment.Secret);
                }

                // Creating a client for the environment
                PayPalHttpClient client = new PayPalHttpClient(environment);
                return client;
            }


            //### Creating an Order
            //This will create an order and print order id for the created order

            // This Method was Adapted from Paypal
            //https://developer.paypal.com/docs/checkout/standard/integrate/
            public async static Task<HttpResponse> createOrder(MyPaypalSetup paypalSetup)
            {
                HttpResponse response = null;

                try
                {
                    // Construct a request object and set desired parameters
                    // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
                    #region order_creation
                    var order = new OrderRequest()
                    {
                        CheckoutPaymentIntent = "CAPTURE",
                        PurchaseUnits = new List<PurchaseUnitRequest>()
                        {
                            new PurchaseUnitRequest()
                            {
                                Items = new List<Item>()
                                {
                                    new Item()
                                    {
                                        Quantity = "1",
                                        Name = "Dog Rescue Fund Donation",
                                        Description = "Fund for rescuing Strays",
                                        Sku = "sku",
                                        Tax = new PayPalCheckoutSdk.Orders.Money(){ CurrencyCode = "USD", Value = "0.15" },
                                        UnitAmount = new PayPalCheckoutSdk.Orders.Money(){ CurrencyCode = "USD", Value = "79.99" }
                                    },
                                    
                                },

                                AmountWithBreakdown = new AmountWithBreakdown()
                                {
                                    CurrencyCode = "USD",
                                    Value = "80.14",

                                    AmountBreakdown = new AmountBreakdown()
                                    {
                                        TaxTotal = new PayPalCheckoutSdk.Orders.Money()
                                        {
                                            CurrencyCode = "USD",
                                            Value = "0.15"
                                        },
                                        ItemTotal = new PayPalCheckoutSdk.Orders.Money()
                                        {
                                            CurrencyCode = "USD",
                                            Value = "79.99"
                                        }
                                    }
                                }
                            }
                        },
                        ApplicationContext = new ApplicationContext()
                        {
                            ReturnUrl = paypalSetup.RedirectUrl,
                            CancelUrl = paypalSetup.RedirectUrl + "&Cancel=true"
                        }
                    };

                    #endregion

                    //IMPORTANT
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                    // Call API with your client and get a response for your call
                    var request = new OrdersCreateRequest();
                    request.Prefer("return=representation");
                    request.RequestBody(order);
                    PayPalHttpClient paypalHttpClient = client(paypalSetup);
                    response = await paypalHttpClient.Execute(request);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                    throw;
                }
                return response;
            }


            //### Capturing an Order
            //Before capturing an order, order should be approved by the buyer using the approve link in create order response

            // This Method was Adapted from Paypal
            //https://developer.paypal.com/docs/checkout/standard/integrate/
            public async static Task<HttpResponse> captureOrder(MyPaypalSetup paypalSetup)
            {
                // Construct a request object and set desired parameters
                // Replace ORDER-ID with the approved order id from create order
                var request = new OrdersCaptureRequest(paypalSetup.PayerApprovedOrderId);
                request.RequestBody(new OrderActionRequest());
                PayPalHttpClient paypalHttpClient = client(paypalSetup);
                HttpResponse response = await paypalHttpClient.Execute(request);
                return response;
            }

            // This Class was Adapted from Paypal
            //https://developer.paypal.com/docs/checkout/standard/integrate/
            public class MyPaypalSetup
            {
                /// <summary>
                /// Provide value sandbox for testing,  provide value live for real money
                /// </summary>
                public String Environment { get; set; }
                /// <summary>
                /// Client id as provided by Paypal on dashboard. Ensure you use correct value based on your environment selection
                /// Use sandbox accounts for sandbox testing
                /// </summary>
                public String ClientId { get; set; }
                /// <summary>
                /// Secret as provided by Paypal on dashboard. Ensure you use correct value based on your environment selection
                /// Use sandbox accounts for sandbox testing
                /// </summary>
                public String Secret { get; set; }

                /// <summary>
                /// This is the URL that you will pass to paypal which paypal will use to redirect payer back to your website.
                /// So essentially it is the same controller URL that you must pass
                /// </summary>
                public String RedirectUrl { get; set; }

                /// <summary>
                /// Once order is created on Paypal, it redirects control to your app with a URL that shows order details. Your website must take the payer to this page
                /// so the payer approved the payment. Store this URL in this property
                /// </summary>
                public String ApproveUrl { get; set; }

                /// <summary>
                /// When paypal redirects control to your website it provides a Approved Order ID which we then pass it back to paypal to execute the order.
                /// Store this approved order ID in this property
                /// </summary>
                public String PayerApprovedOrderId { get; set; }
            }

        }




    }
}