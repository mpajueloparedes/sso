using MaproSSO.Domain.Common;
using MaproSSO.Domain.Enums;
using MaproSSO.Domain.Events.Subscription;
using MaproSSO.Domain.Exceptions;
using MaproSSO.Domain.ValueObjects;

namespace MaproSSO.Domain.Entities.Subscription
{
    public class Payment : BaseEntity
    {
        public Guid SubscriptionId { get; private set; }
        public Money Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public string PaymentMethod { get; private set; }
        public string TransactionId { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string FailureReason { get; private set; }
        public string InvoiceNumber { get; private set; }
        public string InvoiceUrl { get; private set; }
        public Money RefundedAmount { get; private set; }
        public DateTime? RefundedAt { get; private set; }
        public string Notes { get; private set; }

        private Payment() { }

        public static Payment Create(
            Guid subscriptionId,
            Money amount,
            string paymentMethod,
            string invoiceNumber)
        {
            if (amount == null || amount.Amount <= 0)
                throw new DomainException("El monto del pago debe ser mayor a cero");

            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new DomainException("El método de pago es requerido");

            if (string.IsNullOrWhiteSpace(invoiceNumber))
                throw new DomainException("El número de factura es requerido");

            return new Payment
            {
                SubscriptionId = subscriptionId,
                Amount = amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = paymentMethod,
                Status = PaymentStatus.Pending,
                InvoiceNumber = invoiceNumber
            };
        }

        public void MarkAsCompleted(string transactionId, string invoiceUrl = null)
        {
            if (Status != PaymentStatus.Pending)
                throw new BusinessRuleValidationException("Solo los pagos pendientes pueden ser completados");

            if (string.IsNullOrWhiteSpace(transactionId))
                throw new DomainException("El ID de transacción es requerido");

            Status = PaymentStatus.Completed;
            TransactionId = transactionId;
            InvoiceUrl = invoiceUrl;

            AddDomainEvent(new PaymentCompletedEvent(Id, SubscriptionId, Amount.Amount, Amount.Currency));
        }

        public void MarkAsFailed(string failureReason)
        {
            if (Status != PaymentStatus.Pending)
                throw new BusinessRuleValidationException("Solo los pagos pendientes pueden ser marcados como fallidos");

            if (string.IsNullOrWhiteSpace(failureReason))
                throw new DomainException("La razón del fallo es requerida");

            Status = PaymentStatus.Failed;
            FailureReason = failureReason;
        }

        public void Refund(Money refundAmount, string notes = null)
        {
            if (Status != PaymentStatus.Completed)
                throw new BusinessRuleValidationException("Solo los pagos completados pueden ser reembolsados");

            if (refundAmount.Amount > Amount.Amount)
                throw new BusinessRuleValidationException("El monto de reembolso no puede ser mayor al monto pagado");

            if (RefundedAmount != null && (RefundedAmount.Amount + refundAmount.Amount) > Amount.Amount)
                throw new BusinessRuleValidationException("El monto total de reembolsos excede el monto pagado");

            Status = PaymentStatus.Refunded;
            RefundedAmount = RefundedAmount == null ? refundAmount : RefundedAmount.Add(refundAmount);
            RefundedAt = DateTime.UtcNow;
            Notes = notes;
        }

        public bool IsFullyRefunded()
        {
            return RefundedAmount != null && RefundedAmount.Amount == Amount.Amount;
        }
    }
}